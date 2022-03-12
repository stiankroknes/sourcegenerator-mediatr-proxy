using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SourceGenerator.MediatR.Proxy.Tests.Support
{
    internal static class GeneratorRunner
    {
        public static GeneratorResult Run(string sourceCode, string attributeUsage, ISourceGenerator generators)
        {
            var compilation = CreateCompilation(sourceCode, attributeUsage);

            var driver = CSharpGeneratorDriver.Create(
                ImmutableArray.Create(generators), ImmutableArray<AdditionalText>.Empty, (CSharpParseOptions)compilation.SyntaxTrees.First().Options);

            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            var generatedCode = GetGeneratedCode(generators, outputCompilation);

            return new GeneratorResult
            {
                Compilation = compilation,
                Diagnostics = diagnostics,
                WarningMessages = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning).Select(d => d.GetMessage()).ToList(),
                ErrorMessages = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => d.GetMessage()).ToList(),
                GeneratedCode = GetGeneratedCode(generators, outputCompilation),
            };
        }

        private static Compilation CreateCompilation(string source, string attributeUsage)
        {
            var syntaxTrees = new[]
            {
                CSharpSyntaxTree.ParseText(string.Concat(attributeUsage, Environment.NewLine, source), new CSharpParseOptions(LanguageVersion.Preview))
            };

            var references = new List<PortableExecutableReference>();

            foreach (var library in DependencyContext.Default.RuntimeLibraries.Where(lib => lib.Name.Contains("SourceGenerators.")))
            {
                var assembly = Assembly.Load(new AssemblyName(library.Name));
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }

            references.Add(MetadataReference.CreateFromFile(typeof(Contracts.MediatrProxyContractAttribute).GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location));

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(nameof(GeneratorRunner), syntaxTrees, references, options);

            return compilation;
        }

        private static IReadOnlyList<GeneratedSource> GetGeneratedCode(ISourceGenerator generators, Compilation outputCompilation) =>
            outputCompilation.SyntaxTrees
                .Where(syntaxTree => syntaxTree.FilePath.IndexOf(generators.GetType().Name, StringComparison.Ordinal) > -1)
                .Select(t => new GeneratedSource
                {
                    SyntaxTree = t,
                    Source = t.ToString(),
                    FileName = t.FilePath[(t.FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1)..],
                    FilePath = t.FilePath,
                }).ToList();
    }
}