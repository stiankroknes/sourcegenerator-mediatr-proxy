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
        private static readonly DiagnosticDescriptor GeneratorOnDuplicateAttributeUsage = new("SGENMPRX001", "Cannot generate interface", "Duplicate usage of {0} was found in assembly", "SourceGeneratorMediatRProxy", DiagnosticSeverity.Error, true);

        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => new CandidateSyntaxReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not CandidateSyntaxReceiver syntaxReceiver)
            {
                return;
            }

            var compilation = context.Compilation;

            // The source generator will run in two contexts (contract, impl). Consider if better to split in two packages.
            // Project where interface is generated will have the attribute present. We should not add the attribute to project implementing the interface.
            bool isAttributeDefined = context.Compilation.SourceModule.ReferencedAssemblySymbols
                .Any(a => a.GetAttributes()
                    .Any(a => a.AttributeClass.Name == "MediatrProxyContractAttribute"));

            if (!isAttributeDefined)
            {
                // Create a new compilation that contains the attribute.
                var attributesSource = ResourceReader.GetResource("MediatrProxyAttribute.cs", GetType());
                context.AddSource("MediatrAttributes_MainAttributes__", SourceText.From(attributesSource, Encoding.UTF8));
                var options = (CSharpParseOptions)context.Compilation.SyntaxTrees.First().Options;
                var attributeSyntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(attributesSource, Encoding.UTF8), options);
                compilation = context.Compilation.AddSyntaxTrees(attributeSyntaxTree);
            }

            // Get attributes
            var proxyImplementationAttribute = compilation.GetTypeByMetadataName("SourceGenerator.MediatR.Proxy.MediatrProxyImplementationAttribute");
            var proxyContractAttribute = compilation.GetTypeByMetadataName("SourceGenerator.MediatR.Proxy.MediatrProxyContractAttribute");

            // Extract attribute usage options
            var proxyContractOptions = GetProxyContractOptions(context, compilation, proxyContractAttribute);
            var proxyImplementationOptions = GetProxyImplementationOptions(context, compilation, proxyImplementationAttribute);

            if (proxyContractOptions != null)
            {
                GenerateInterface(context, syntaxReceiver, proxyContractOptions, proxyImplementationOptions);
            }
            else if (proxyImplementationOptions != null)
            {
                GenerateInterfaceImplementation(context, proxyImplementationOptions, compilation);
            }
        }

        private static void GenerateInterface(GeneratorExecutionContext context, CandidateSyntaxReceiver syntaxReceiver, ProxyContractOptions proxyContractOptions, ProxyImplementationOptions proxyImplementationOptions)
        {
            // Scan contract assembly for all query/commands.
            var requests = MediatrRequestScanner.GetAll(syntaxReceiver.CandidateTypes, proxyContractOptions);

            if (requests.Count == 0)
            {
                // diagnostics?
                return;
            }

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

        private static void GenerateInterfaceImplementation(GeneratorExecutionContext context, ProxyImplementationOptions proxyImplementationOptions, Compilation compilation)
        {
            // TODO: resolve contract assembly using context.Compilation.SourceModule.ReferencedAssemblySymbols and use the scanner logic?

            // Find the proxy interface we should implement. 
            var proxyInterfaceName = $"{proxyImplementationOptions.ContractNamespace}.{proxyImplementationOptions.ProxyInterfaceName}";
            var proxyInterfaceType = compilation.GetTypeByMetadataName(proxyInterfaceName);

            // Get request details about each method in the interface.
            var requests = proxyInterfaceType.GetMembers()
                .OfType<IMethodSymbol>()
                .Select(m =>
                {
                    bool isQuery = m.Parameters[0].ToString().EndsWith(proxyImplementationOptions.QueryPostfix);

                    var name = m.Parameters[0].Type.Name.StripPostfix(isQuery
                        ? proxyImplementationOptions.QueryPostfix
                        : proxyImplementationOptions.CommandPostfix);

                    return new RequestDetail
                    {
                        Name = name,
                        Type = m.Parameters[0].ToString(),
                        ReturnType = m.ReturnType.ToString(),
                        Namespace = m.Parameters[0].ContainingNamespace?.ToString(),
                        IsQuery = isQuery
                    };
                }).ToList();

            var source = SourceCodeGenerator.GenerateImplementation(proxyImplementationOptions, requests);
            context.AddSource(source.FileName, SourceText.From(source.SourceCode, Encoding.UTF8));
        }

        private static ProxyContractOptions GetProxyContractOptions(GeneratorExecutionContext context, Compilation compilation, INamedTypeSymbol proxyContractAttribute)
        {
            var attributes = compilation.Assembly.GetAttributes()
                .Where(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, proxyContractAttribute))
                .ToList();

            if (attributes.Count > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(GeneratorOnDuplicateAttributeUsage, null, proxyContractAttribute.Name));
            }

            if (attributes.Count == 0)
            {
                return null;
            }

            var attribute = attributes[0];

            var option = new ProxyContractOptions
            {
                ProxyInterfaceName = (string)attribute.ConstructorArguments[0].Value!,
                ContractNamespace = (string)attribute.ConstructorArguments[1].Value!,
            };

            ApplyScannerOptionsFromNamedArguments(option, attribute);

            return option;
        }

        private static ProxyImplementationOptions GetProxyImplementationOptions(GeneratorExecutionContext context, Compilation compilation, INamedTypeSymbol proxyImplementationAttribute)
        {
            var attributes = compilation.Assembly.GetAttributes()
              .Where(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, proxyImplementationAttribute))
              .ToList();

            if (attributes.Count > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(GeneratorOnDuplicateAttributeUsage, null, proxyImplementationAttribute.Name));
            }

            if (attributes.Count == 0)
            {
                return null;
            }

            var attribute = attributes[0];
            var option = new ProxyImplementationOptions
            {
                ProxyInterfaceName = (string)attribute.ConstructorArguments[0].Value!,
                ContractNamespace = (string)attribute.ConstructorArguments[1].Value!,
                ImplementationNamespace = (string)attribute.ConstructorArguments[2].Value!
            };

            ApplyScannerOptionsFromNamedArguments(option, attribute);

            return option;
        }

        private static void ApplyScannerOptionsFromNamedArguments(MediatrRequestScannerOptions options, AttributeData attribute)
        {
            foreach (KeyValuePair<string, TypedConstant> arg in attribute.NamedArguments)
            {
                switch (arg.Key)
                {
                    case nameof(MediatrRequestScannerOptions.QueryIdentifierString):
                        options.QueryIdentifierString = GetStringValue(arg) ?? options.QueryIdentifierString;
                        break;
                    case nameof(MediatrRequestScannerOptions.CommandIdentifierString):
                        options.CommandIdentifierString = GetStringValue(arg) ?? options.CommandIdentifierString;
                        break;
                    case nameof(MediatrRequestScannerOptions.QueryPostfix):
                        options.QueryPostfix = GetStringValue(arg) ?? options.QueryPostfix;
                        break;
                    case nameof(MediatrRequestScannerOptions.CommandPostfix):
                        options.CommandPostfix = GetStringValue(arg) ?? options.CommandPostfix;
                        break;
                }
            }

            static string GetStringValue(KeyValuePair<string, TypedConstant> arg) =>
                arg.Value.Value is string s ? s : default;
        }
    }
}