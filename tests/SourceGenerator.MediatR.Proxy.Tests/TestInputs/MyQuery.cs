namespace SourceGenerator.MediatR.Proxy.Tests.TestInputs
{
    /// <summary>
    /// This is my test query.
    /// </summary>
    public class MyQuery : Query<MyQueryResultType>
    {
        public string Argument { get; set; }
    }

    public class MyQueryResultType { 
    
        public string Value { get; set; }
    }
}