using MediatR;

var dummyMediator = new DummyMediator();
MyApp.Shared.IMyService service = new MyApp.Application.MyService(dummyMediator);

var generateSomethingCommand = new MyApp.Shared.GenerateSomethingCommand();
await service.GenerateSomething(generateSomethingCommand);
Console.WriteLine("OK? " + dummyMediator.Sent.Any(t => t == generateSomethingCommand));

var getSomeDataQuery = new MyApp.Shared.GetSomeDataQuery();
await service.GetSomeData(getSomeDataQuery);
Console.WriteLine("OK? " + dummyMediator.Sent.Any(t => t == getSomeDataQuery));

MyApp.SecondShared.IMyOtherService service2 = new MyApp.Application.MyOtherService(dummyMediator);
var getSome2 = new MyApp.SecondShared.GetSomeDataQuery();
await service2.GetSomeData(getSome2);
Console.WriteLine("OK? " + dummyMediator.Sent.Any(t => t == getSome2));

class DummyMediator : IMediator
{
    private readonly List<IBaseRequest> sent = new();

    public IReadOnlyCollection<IBaseRequest> Sent => sent;

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        throw new NotImplementedException();
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        sent.Add((IBaseRequest)request);
        TResponse response = default!;
        return Task.FromResult(response);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        throw new NotImplementedException();
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        sent.Add((IBaseRequest)request);
        return Task.FromResult((object?)null);
    }
}