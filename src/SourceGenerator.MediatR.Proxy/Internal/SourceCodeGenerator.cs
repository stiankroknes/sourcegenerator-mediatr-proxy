﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceGenerator.MediatR.Proxy.Internal
{
    /// <summary>
    /// Responsible for generating source code for interface and implementation of interface.
    /// </summary>
    internal static class SourceCodeGenerator
    {
        private static readonly Type thisAssemblyType = typeof(SourceCodeGenerator);

        // TODO: add support custom template, maybe split to usings, attributes, method templates, and allow override parts.
        // Now we generate with servicemodel attributes.

        internal static SourceFile GenerateInterface(ProxyContractOptions proxyContractOptions, IReadOnlyList<RequestDetail> requests)
        {
            var templateString = ResourceReader.GetResource("ServiceInterface.scriban", thisAssemblyType);

            string name = proxyContractOptions.ProxyInterfaceName;
            string rootNamespace = proxyContractOptions.ContractNamespace;

            // TODO allow define default usings in config.
            var usings = new[]
                {
                    "System.Collections.Generic",
                    "System.Threading",
                    "System.Threading.Tasks",
                }
                .Concat(requests.Select(c => c.Namespace))
                .Distinct().ToList();

            var model = new ContractTemplateModel
            {
                InterfaceModifier = "public",
                Commands = requests.Where(r => r.IsCommand).ToList(),
                Queries = requests.Where(r => r.IsQuery).ToList(),
                InterfaceName = name,
                Namespace = rootNamespace,
                Usings = usings,
            };

            var result = TemplateEngine.Execute(templateString, model);

            return new SourceFile($"{model.InterfaceName}.g.cs", result);
        }

        internal static SourceFile GenerateImplementation(ProxyImplementationOptions proxyOptions, IReadOnlyList<RequestDetail> requests)
        {
            var templateString = ResourceReader.GetResource("ServiceImplementation.scriban", thisAssemblyType);

            // TODO name by csproj prop ? attribute ?
            string name = proxyOptions.ProxyInterfaceName.Substring(1); // TODO more robust strategy. 
            string rootNamespace = proxyOptions.ImplementationNamespace;

            // TODO allow define default usings in config.
            var usings = new[]
                {
                    "System.Collections.Generic",
                    "System.Threading",
                    "System.Threading.Tasks",
                    "MediatR",
                    proxyOptions.ContractNamespace
                }
                .Concat(requests.Select(c => c.Namespace)).Distinct().ToList();

            var model = new ImplementationTemplateModel
            {
                ClassModifier = "public",
                Commands = requests.Where(r => r.IsCommand).ToList(),
                Queries = requests.Where(r => r.IsQuery).ToList(),
                ClassName = name,
                InterfaceName = proxyOptions.ProxyInterfaceName,
                Namespace = rootNamespace,
                Usings = usings,
            };

            var result = TemplateEngine.Execute(templateString, model);

            return new SourceFile($"{model.ClassName}.g.cs", result);
        }
    }
}