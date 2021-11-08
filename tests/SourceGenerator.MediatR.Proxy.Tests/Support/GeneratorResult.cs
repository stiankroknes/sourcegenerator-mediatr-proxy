using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SourceGenerator.MediatR.Proxy.Tests.Support
{
    internal class GeneratorResult
    {
        public Compilation Compilation { get; init; }
        public IReadOnlyList<string> ErrorMessages { get; init; }
        public IReadOnlyList<string> WarningMessages { get; init; }
        public ImmutableArray<Diagnostic> Diagnostics { get; init; }
        public IReadOnlyList<GeneratedSource> GeneratedCode { get; init; }
    }

    internal class GeneratedSource
    {
        public SyntaxTree SyntaxTree { get; init; }
        public string Source { get; init; }
        public string FilePath { get; init; }
        public string FileName { get; init; }
    }

    internal static class GeneratorResultExtensions
    {
        internal static GeneratedSource GetSourceByFileName(this GeneratorResult result, string filename) =>
            result.GeneratedCode.SingleOrDefault(g => string.Equals(filename, g.FileName, StringComparison.Ordinal));
    }
}