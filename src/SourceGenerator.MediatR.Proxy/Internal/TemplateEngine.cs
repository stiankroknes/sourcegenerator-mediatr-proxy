﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Scriban;
using System;
using System.Linq;

namespace SourceGenerator.MediatR.Proxy.Internal
{
    internal static class TemplateEngine
    {
        public static string Execute(string templateString, object model)
        {
            var template = Template.Parse(templateString);

            if (template.HasErrors)
            {
                var errors = string.Join(" | ", template.Messages.Select(x => x.Message));
                throw new InvalidOperationException($"Template parse error: {template.Messages}");
            }

            var result = template.Render(model, member => member.Name);

            result = SyntaxFactory.ParseCompilationUnit(result)
                .NormalizeWhitespace()
                .GetText()
                .ToString();

            return result;
        }
    }
}