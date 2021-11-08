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
        public string QueryIdentifierString { get; set; } = "Query";
        public string CommandIdentifierString { get; set; } = "Command";

        public string QueryPostfix { get; set; } = "Query";
        public string CommandPostfix { get; set; } = "Command";
    }
}