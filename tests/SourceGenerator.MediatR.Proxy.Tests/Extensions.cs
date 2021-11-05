using System;
using System.Text.RegularExpressions;
using Xunit;

namespace SourceGenerator.MediatR.Proxy.Tests
{
    public static class Extensions
    {
        public static void AssertSourceCodesEquals(this string expected, string actual) =>
            Assert.Equal(expected.TrimWhiteSpaces(), actual.TrimWhiteSpaces());

        public static string TrimWhiteSpaces(this string text) =>
            text == null
                ? text
                : Regex.Replace(text, @"\s+", string.Empty)
                    .Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
    }
}