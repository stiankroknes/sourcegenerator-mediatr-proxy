using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SourceGenerator.MediatR.Proxy.Internal
{
    internal static class ResourceReader
    {
        public static string GetResource(string endWith, Type assemblyType = null)
        {
            var assembly = GetAssembly(assemblyType);

            var resources = assembly.GetManifestResourceNames()
                .Where(r => r.EndsWith(endWith))
                .ToList();

            if (resources.Count == 0)
            {
                throw new InvalidOperationException($"There is no resources that ends with '{endWith}'");
            }

            if (resources.Count > 1)
            {
                throw new InvalidOperationException($"There is more then one resource that ends with '{endWith}'");
            }

            var resourceName = resources.Single();
            return ReadEmbeddedResource(assembly, resourceName);
        }

        private static Assembly GetAssembly(Type assemblyType) =>
            assemblyType == null
                ? Assembly.GetExecutingAssembly()
                : Assembly.GetAssembly(assemblyType);

        private static string ReadEmbeddedResource(Assembly assembly, string name)
        {
            using var resourceStream = assembly.GetManifestResourceStream(name);

            if (resourceStream == null)
            {
                return null;
            }

            using var streamReader = new StreamReader(resourceStream);

            return streamReader.ReadToEnd();
        }
    }
}