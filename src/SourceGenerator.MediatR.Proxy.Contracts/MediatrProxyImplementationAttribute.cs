namespace SourceGenerator.MediatR.Proxy.Contracts;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class MediatrProxyImplementationAttribute : Attribute
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
