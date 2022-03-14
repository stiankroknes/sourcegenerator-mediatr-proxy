using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenerator.MediatR.Proxy
{
    internal static class Extensions
    {
        public static string GetNamespace(this BaseTypeDeclarationSyntax syntax)
        {
            string nameSpace = string.Empty;
            SyntaxNode potentialNamespaceParent = syntax.Parent;

            while (potentialNamespaceParent != null &&
                    potentialNamespaceParent is not NamespaceDeclarationSyntax
                    && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                nameSpace = namespaceParent.Name.ToString();

                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            return nameSpace;
        }

        public static CompilationUnitSyntax GetCompilationUnit(this SyntaxNode syntaxNode) =>
            syntaxNode.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();

        public static List<string> GetUsings(this CompilationUnitSyntax root) => root.ChildNodes()
                .OfType<UsingDirectiveSyntax>()
                .Select(n => n.Name.ToString())
                .ToList();

        public static string StripPostfix(this string requestName, string postfixToStrip)
        {
            int postfixIndex = requestName.LastIndexOf(postfixToStrip);
            return postfixIndex > 0
                ? requestName.Substring(0, postfixIndex)
                : requestName;
        }
    }
}
