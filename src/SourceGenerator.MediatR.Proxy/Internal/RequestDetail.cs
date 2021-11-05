namespace SourceGenerator.MediatR.Proxy.Internal
{
    /// <summary>
    /// Details about a given command or query type.
    /// </summary>
    internal record RequestDetail
    {
        public string Name { get; init; }
        public string Type { get; init; }
        public string Namespace { get; init; }
        public string ReturnType { get; init; }
        public string Comments { get; init; }
        public bool IsQuery { get; init; }
        public bool IsCommand => !IsQuery;
    }
}