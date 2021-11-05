namespace SourceGenerator.MediatR.Proxy.Tests.TestInputs
{
    /// <summary>
    /// This is my test command.
    /// </summary>
    public class MyCommand : Command<MyCommandResultType>
    {
        public string Argument { get; set; }
    }


    public class MyCommandResultType
    {
        public string Value { get; set; }
    }
}