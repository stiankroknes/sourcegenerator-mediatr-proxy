namespace SourceGenerator.MediatR.Proxy.Contracts;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class MediatrProxyContractAttribute : Attribute
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
