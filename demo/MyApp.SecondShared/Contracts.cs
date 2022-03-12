namespace MyApp.SecondShared
{
    using MediatR;

    public abstract class RequestBase<TResponse> : IRequest<TResponse>
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString("D");
    }

    public abstract class Command<TResponse> : RequestBase<TResponse> { }

    public abstract class Query<TResponse> : RequestBase<TResponse> { }

    public class GetSomeDataQuery : Query<SomeDataResult>
    {
    }

    public class SomeDataResult
    {
        public string? Data { get; set; }
    }
}