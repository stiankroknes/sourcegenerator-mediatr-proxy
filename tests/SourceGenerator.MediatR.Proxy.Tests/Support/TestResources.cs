using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SourceGenerator.MediatR.Proxy.Tests.Support
{
    internal static class TestResources
    {
        private static readonly Assembly ThisAssembly = typeof(TestResources).Assembly;
        private static readonly string RootNamespace = typeof(Extensions).Namespace;

        private static readonly ConcurrentDictionary<string, Task<string>> Cache = new();

        public static Task<string> GetTestInput(string filename) =>
            GetManifestResource("TestInputs", filename);

        public static Task<string> GetTestOutputs(string filename) =>
            GetManifestResource("TestOutputs", filename);

        private static Task<string> GetManifestResource(string folder, string filename) =>
                Cache.GetOrAdd($"{folder}.{filename}", key => ReadManifestResource($"{RootNamespace}.{key}"));

        private static async Task<string> ReadManifestResource(string manifestResourceName)
        {
            var stream = ThisAssembly.GetManifestResourceStream(manifestResourceName);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}