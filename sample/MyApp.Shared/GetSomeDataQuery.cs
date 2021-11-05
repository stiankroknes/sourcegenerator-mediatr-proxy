namespace MyApp.Shared
{
    public class GetSomeDataQuery : Query<SomeDataResult>
    {
    }

    public class SomeDataResult
    {
        public string? Data { get; set; }
    }
}