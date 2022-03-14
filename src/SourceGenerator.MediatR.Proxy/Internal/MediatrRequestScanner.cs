using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace SourceGenerator.MediatR.Proxy.Internal
{

    /// <summary>
    /// Scanner to identify all queries and commands from the list of candidate types detected in the Initialize phase.
    /// </summary>
    internal static class MediatrRequestScanner
    {
        public static IReadOnlyList<RequestDetail> GetAll(Compilation compilation, List<TypeDeclarationSyntax> candidateTypeDeclarationSyntaxes,
            MediatrRequestScannerOptions options)
        {
            var requests = new List<RequestDetail>();

            foreach (var tds in candidateTypeDeclarationSyntaxes)
            {
                var model = compilation.GetSemanticModel(tds.SyntaxTree);

                var ns = tds.GetNamespace();

                foreach (var entry in tds.BaseList.Types)
                {
                    if (entry is not SimpleBaseTypeSyntax { Type: GenericNameSyntax type } baseType)
                    {
                        continue;
                    }

                    bool isQuery = Identify(type, options.QueryIdentifierString);
                    bool isCommand = Identify(type, options.CommandIdentifierString);

                    if (isQuery || isCommand)
                    {
                        var request = tds;

                        var returnType = GetReturnTypeFromGenericArgs(model, request, isQuery ? options.QueryIdentifierString : options.CommandIdentifierString);
                        var comments = request.GetLeadingTrivia().ToString();
                        var requestNamespace = ((QualifiedNameSyntax)((BaseNamespaceDeclarationSyntax)request.Parent).Name).ToString();
                        var requestName = request.Identifier.ValueText.StripPostfix(isQuery ? options.QueryPostfix : options.CommandPostfix);

                        if (string.Equals(ns, requestNamespace, System.StringComparison.Ordinal))
                        {
                            requestNamespace = ns;
                        }

                        requests.Add(new RequestDetail
                        {
                            Name = requestName,
                            Type = $"{requestNamespace}.{request.Identifier.ValueText}",
                            IsQuery = isQuery,
                            ReturnType = $"System.Threading.Tasks.Task<{returnType}>",
                            Namespace = requestNamespace,
                            Comments = comments
                        });

                        break;
                    }
                }
            }

            return requests;

            static bool Identify(GenericNameSyntax type, string identifier) =>
                type.Identifier.ValueText == identifier &&
                type.TypeArgumentList.Arguments.Count == 1 &&
                (type.TypeArgumentList.Arguments[0] is not null);
        }

        private static string GetReturnTypeFromGenericArgs(SemanticModel model, TypeDeclarationSyntax command, string baseIdentifier)
        {
            foreach (var entry in command.BaseList.Types)
            {
                if (entry is SimpleBaseTypeSyntax basetype && basetype.Type is GenericNameSyntax type)
                {
                    if (type.Identifier.ValueText == baseIdentifier && type.TypeArgumentList.Arguments.Count == 1)
                    {
                        var typeInfo = model.GetTypeInfo(type.TypeArgumentList.Arguments[0]);
                        var nameSpace = ((INamedTypeSymbol)typeInfo.Type).ContainingNamespace;
                        return $"{nameSpace.ToDisplayString()}.{type.TypeArgumentList.Arguments[0]}";
                    }
                }
            }

            return string.Empty;
        }
    }
}