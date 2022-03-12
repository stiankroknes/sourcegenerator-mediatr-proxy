using MediatR;

var dummyMediator = new DummyMediator();
MyApp.Shared.IMyService service = new MyApp.Application.MyService(dummyMediator);

var generateSomethingCommand = new MyApp.Shared.GenerateSomethingCommand();
service.GenerateSomething(generateSomethingCommand);
Console.WriteLine("OK? " + dummyMediator.Sent.Any(t => t == generateSomethingCommand));

var getSomeDataQuery = new MyApp.Shared.GetSomeDataQuery();
service.GetSomeData(getSomeDataQuery);
Console.WriteLine("OK? " + dummyMediator.Sent.Any(t => t == getSomeDataQuery));

class DummyMediator : IMediator
{
    private readonly List<IBaseRequest> sent = new();

    public IReadOnlyCollection<IBaseRequest> Sent => sent;

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task Publish(object notification, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification =>
        throw new NotImplementedException();

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        sent.Add(request);
        return Task.FromResult(Activator.CreateInstance<TResponse>());
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        sent.Add((IBaseRequest)request);
        return Task.FromResult((object?)null);
    }
}