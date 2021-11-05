namespace SourceGenerator.MediatR.Proxy.Internal
{
    internal class ProxyContractOptions : MediatrRequestScannerOptions
    {
        public string ProxyInterfaceName { get; init; }
        public string ContractNamespace { get; init; }
    }

    internal class ProxyImplementationOptions : MediatrRequestScannerOptions
    {
        public string ProxyInterfaceName { get; init; }
        public string ContractNamespace { get; init; }
        public string ImplementationNamespace { get; init; }
    }
    
    internal class MediatrRequestScannerOptions
    {
        public string QueryIdentifierString { get; init; } = "Query";
        public string CommandIdentifierString { get; init; } = "Command";

        public string QueryPostfix { get; init; } = "Query";
        public string CommandPostfix { get; init; } = "Command";
    }
}