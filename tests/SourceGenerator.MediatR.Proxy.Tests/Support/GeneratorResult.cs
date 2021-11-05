using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SourceGenerator.MediatR.Proxy.Tests.Support
{
    internal class GeneratorResult
    {
        public GeneratorResult(Compilation Compilation, ImmutableArray<Diagnostic> Diagnostics, IEnumerable<GeneratedSource> generatedCode)
        {
            this.Compilation = Compilation;
            this.Diagnostics = Diagnostics;
            GeneratedCode = generatedCode;
        }

        public Compilation Compilation { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public IEnumerable<GeneratedSource> GeneratedCode { get; }
    }

    internal class GeneratedSource
    {
        public string Source { get; set; }
        public string FilePath  { get; set; }
        public string FileName { get; set; }

    }
}