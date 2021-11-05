using System.Collections.Generic;

namespace SourceGenerator.MediatR.Proxy.Internal
{
    internal class ContractTemplateModel
    {
        public string Namespace { get; init; }
        public string InterfaceModifier { get; init; }
        public string InterfaceName { get; init; }

        public IEnumerable<string> Usings { get; init; }

        public IEnumerable<RequestDetail> Commands { get; init; }

        public IEnumerable<RequestDetail> Queries { get; init; }
    }

    internal class ImplementationTemplateModel
    {
        public string Namespace { get; init; }
        public string ClassModifier { get; init; }
        public string ClassName { get; init; }
        public string InterfaceName { get; init; }

        public IEnumerable<string> Usings { get; init; }

        public IEnumerable<RequestDetail> Commands { get; init; }

        public IEnumerable<RequestDetail> Queries { get; init; }
    }
}