using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SourceGenerator.MediatR.Proxy.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceGenerator.MediatR.Proxy
{
    /// <summary>
    /// Created on demand before each generation pass
    /// </summary>
    internal class CandidateSyntaxReceiver : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> CandidateTypes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not (ClassDeclarationSyntax or RecordDeclarationSyntax))
            {
                return;
            }

            var typeDeclarationSyntax = (TypeDeclarationSyntax)syntaxNode;
            
            if (typeDeclarationSyntax.BaseList == null)
            {
                return;
            }

            CandidateTypes.Add(typeDeclarationSyntax);
        }
    }

    [Generator]
    public class MediatrProxyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => new CandidateSyntaxReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not CandidateSyntaxReceiver syntaxReceiver)
            {
                return;
            }
            
            // Create a new compilation that contains the attribute.
            var attributesSource = ResourceReader.GetResource("MediatrProxyAttribute.cs", GetType());
            context.AddSource("MediatrAttributes_MainAttributes__", SourceText.From(attributesSource, Encoding.UTF8));
            var options = (CSharpParseOptions)context.Compilation.SyntaxTrees.First().Options;
            var attributeSyntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(attributesSource, Encoding.UTF8), options);
            var compilation = context.Compilation.AddSyntaxTrees(attributeSyntaxTree);

            // Get attributes
            var proxyImplementationAttribute = compilation.GetTypeByMetadataName("MediatrProxyImplementationAttribute");
            var proxyContractAttribute = compilation.GetTypeByMetadataName("MediatrProxyContractAttribute");

            // Extract attribute usage options
            var proxyContractOptions = GetProxyContractOptions(compilation, proxyContractAttribute);
            var proxyImplementationOptions = GetProxyImplementationOptions(compilation, proxyImplementationAttribute);

            MediatrRequestScannerOptions scannerOptions = proxyContractOptions is not null ? proxyContractOptions : proxyImplementationOptions;

            // Scan contract assembly for all query/commands.
            var requests = MediatrRequestScanner.GetAll(syntaxReceiver.CandidateTypes, scannerOptions);

            if (requests.Count == 0)
            {
                return;
            }

            if (proxyContractOptions != null)
            {
                GenerateInterface(context, requests, proxyContractOptions, proxyImplementationOptions);
            }
            else if (proxyImplementationOptions != null)
            {
                GenerateInterfaceImplementation(context, requests, proxyImplementationOptions, compilation);
            }
        }

        private static void GenerateInterface(GeneratorExecutionContext context, IReadOnlyList<RequestDetail> requests, ProxyContractOptions proxyContractOptions, ProxyImplementationOptions proxyImplementationOptions)
        {
            // Generate interface containing a method for each query/command found.
            var source = SourceCodeGenerator.GenerateInterface(proxyContractOptions, requests);
            context.AddSource(source.FileName, SourceText.From(source.SourceCode, Encoding.UTF8));

            // Should we also generate implementation of the interface?
            if (proxyImplementationOptions != null)
            {
                var implementationSource = SourceCodeGenerator.GenerateImplementation(proxyImplementationOptions, requests);
                context.AddSource(implementationSource.FileName, SourceText.From(implementationSource.SourceCode, Encoding.UTF8));
            }
        }

        private static void GenerateInterfaceImplementation(GeneratorExecutionContext context, IReadOnlyList<RequestDetail> requests, ProxyImplementationOptions proxyImplementationOptions, Compilation compilation)
        {
            // Find the proxy interface we should implement. 
            // var proxyInterfaceName = $"{proxyImplementationOptions.ContractNamespace}.{proxyImplementationOptions.ProxyInterfaceName}";
            // var proxyInterfaceType = compilation.GetTypeByMetadataName(proxyInterfaceName);

            // TODO: consider add proxyInterfaceType != null check. 

            // This approach requires the interface to be present... good thing I guess. But we must resolve RequestDetail thru interface instead of using the contracts.

            // Get request details about each method in the interface.
            // var requestDetails = proxyInterfaceType.GetMembers()
            //     .OfType<IMethodSymbol>()
            //     .Select(m =>
            //     {
            //         bool isQuery = m.Parameters[0].ToString().EndsWith(proxyImplementationOptions.QueryPostfix);
            //         var name = TransformRequestTypeToMethodName(proxyImplementationOptions, isQuery, m.Parameters[0].Type.Name);
            //
            //         return new RequestDetail
            //         {
            //             Name = name,
            //             Type = m.Parameters[0].ToString(),
            //             ReturnType = m.ReturnType.ToString(),
            //             Namespace = m.Parameters[0].ContainingNamespace?.ToString(),
            //             IsQuery = isQuery
            //         };
            //     }).ToList();

            var source = SourceCodeGenerator.GenerateImplementation(proxyImplementationOptions, requests);
            context.AddSource(source.FileName, SourceText.From(source.SourceCode, Encoding.UTF8));

            // static string TransformRequestTypeToMethodName(MediatrRequestScannerOptions options, bool isQuery, string requestType)
            // {
            //     var name = requestType;
            //     if (isQuery)
            //     {
            //         var queryPostfixIndex = name.LastIndexOf(options.QueryPostfix, StringComparison.Ordinal);
            //         if (queryPostfixIndex > 0)
            //         {
            //             name = name.Substring(0, queryPostfixIndex);
            //         }
            //     }
            //     else
            //     {
            //         var commandPostfixIndex = name.LastIndexOf(options.CommandPostfix, StringComparison.Ordinal);
            //         if (commandPostfixIndex > 0)
            //         {
            //             name = name.Substring(0, commandPostfixIndex);
            //         }
            //     }
            //
            //     return name;
            // }
        }

        private static ProxyContractOptions GetProxyContractOptions(Compilation compilation, INamedTypeSymbol proxyContractAttribute)
        {
            return compilation.Assembly.GetAttributes()
                .Where(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, proxyContractAttribute))
                .Select(x => new ProxyContractOptions
                {
                    ProxyInterfaceName = (string)x.ConstructorArguments[0].Value!,
                    ContractNamespace = (string)x.ConstructorArguments[1].Value!,
                    QueryIdentifierString = (string)x.ConstructorArguments[2].Value!,
                    CommandIdentifierString = (string)x.ConstructorArguments[3].Value!,
                    QueryPostfix = (string)x.ConstructorArguments[4].Value!,
                    CommandPostfix = (string)x.ConstructorArguments[5].Value!
                })
                .SingleOrDefault();
        }

        private static ProxyImplementationOptions GetProxyImplementationOptions(Compilation compilation, INamedTypeSymbol proxyImplementationAttribute)
        {
            return compilation.Assembly.GetAttributes()
                .Where(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, proxyImplementationAttribute))
                .Select(x => new ProxyImplementationOptions
                {
                    ProxyInterfaceName = (string)x.ConstructorArguments[0].Value!,
                    ContractNamespace = (string)x.ConstructorArguments[1].Value!,
                    ImplementationNamespace = (string)x.ConstructorArguments[2].Value!,
                    QueryIdentifierString = (string)x.ConstructorArguments[3].Value!,
                    CommandIdentifierString = (string)x.ConstructorArguments[4].Value!,
                    QueryPostfix = (string)x.ConstructorArguments[5].Value!,
                    CommandPostfix = (string)x.ConstructorArguments[6].Value!
                })
                .SingleOrDefault();
        }
    }
}