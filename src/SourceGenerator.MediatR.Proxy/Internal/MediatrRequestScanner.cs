using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace SourceGenerator.MediatR.Proxy.Internal
{
    /// <summary>
    /// Scanner to identify all queries and commands from the list of candidate types detected in the Initialize phase.
    /// </summary>
    internal static class MediatrRequestScanner
    {
        public static IReadOnlyList<RequestDetail> GetAll(List<TypeDeclarationSyntax> candidateTypeDeclarationSyntaxes,
            MediatrRequestScannerOptions options)
        {
            var requests = new List<RequestDetail>();

            foreach (var tds in candidateTypeDeclarationSyntaxes)
            {
                var baseList = tds.BaseList;

                foreach (var entry in baseList.Types)
                {
                    if (entry is not SimpleBaseTypeSyntax { Type: GenericNameSyntax type } baseType)
                    {
                        continue;
                    }

                    if (Identify(type, options.CommandIdentifierString))
                    {
                        var command = tds;
                        var commandReturnType = GetReturnTypeFromGenericArgs(command, options.CommandIdentifierString);
                        var comments = command.GetLeadingTrivia().ToString();

                        var commandNamespace = ((QualifiedNameSyntax)((NamespaceDeclarationSyntax)command.Parent).Name).ToString();
                        var commandName = command.Identifier.ValueText.StripPostfix(options.CommandPostfix);

                        requests.Add(new RequestDetail
                        {
                            Name = commandName,
                            Type = $"{commandNamespace}.{command.Identifier.ValueText}",
                            IsQuery = false,
                            ReturnType = $"System.Threading.Tasks.Task<{commandReturnType}>",
                            Namespace = commandNamespace,
                            Comments = comments
                        });

                        break;
                    }

                    if (Identify(type, options.QueryIdentifierString))
                    {
                        var query = tds;
                        var queryReturnType = GetReturnTypeFromGenericArgs(query, options.QueryIdentifierString);
                        var comments = query.GetLeadingTrivia().ToString();

                        var queryNamespace = ((QualifiedNameSyntax)((NamespaceDeclarationSyntax)query.Parent).Name).ToString();
                        var queryName = query.Identifier.ValueText.StripPostfix(options.QueryPostfix);

                        requests.Add(new RequestDetail
                        {
                            Name = queryName,
                            Type = $"{queryNamespace}.{query.Identifier.ValueText}",
                            IsQuery = true,
                            ReturnType = $"System.Threading.Tasks.Task<{queryReturnType}>",
                            Namespace = queryNamespace,
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

        private static string GetReturnTypeFromGenericArgs(TypeDeclarationSyntax command, string baseIdentifier)
        {
            foreach (var entry in command.BaseList.Types)
            {
                if (entry is SimpleBaseTypeSyntax basetype && basetype.Type is GenericNameSyntax type)
                {
                    if (type.Identifier.ValueText == baseIdentifier && type.TypeArgumentList.Arguments.Count == 1)
                    {
                        var p = type.TypeArgumentList.Arguments[0].Parent;
                        return type.TypeArgumentList.Arguments[0].ToString();
                    }
                }
            }

            return string.Empty;
        }
    }
}