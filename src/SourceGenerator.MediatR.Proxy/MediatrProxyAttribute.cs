﻿namespace SourceGenerator.MediatR.Proxy
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class MediatrProxyImplementationAttribute : System.Attribute
    {
        public string ProxyInterfaceName { get; }
        public string ContractNamespace { get; }
        public string ImplementationNamespace { get; }

        public string QueryIdentifierString { get; set; } = "Query";
        public string CommandIdentifierString { get; set; } = "Command";

        public string QueryPostfix { get; set; } = "Query";
        public string CommandPostfix { get; set; } = "Command";

        public MediatrProxyImplementationAttribute(string proxyInterfaceName, string contractNamespace, string implementationNamespace)
        {
            ProxyInterfaceName = proxyInterfaceName;
            ContractNamespace = contractNamespace;
            ImplementationNamespace = implementationNamespace;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class MediatrProxyContractAttribute : System.Attribute
    {
        public string ProxyInterfaceName { get; }
        public string ContractNamespace { get; }

        public string QueryIdentifierString { get; set; } = "Query";
        public string CommandIdentifierString { get; set; } = "Command";

        public string QueryPostfix { get; set; } = "Query";
        public string CommandPostfix { get; set; } = "Command";

        public MediatrProxyContractAttribute(string proxyInterfaceName, string contractNamespace)
        {
            ProxyInterfaceName = proxyInterfaceName;
            ContractNamespace = contractNamespace;
        }
    }
}