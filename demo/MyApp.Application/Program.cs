using MediatR;

MyApp.Shared.IMyService service = new MyApp.Application.MyService(new DummyMediator());

try
{
    service.GenerateSomething(new MyApp.Shared.GenerateSomethingCommand());
}
catch(DummyMediatorException) { Console.WriteLine("This is working as expected. Command is proxied/forwarded to IMediator."); }

try
{
    service.GetSomeData(new MyApp.Shared.GetSomeDataQuery());
}
catch (DummyMediatorException) { Console.WriteLine("This is working as expected. Query is proxied/forwarded to IMediator! "); }


class DummyMediator : IMediator
{
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
        throw new DummyMediatorException();
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        throw new DummyMediatorException();
    }
}

class DummyMediatorException : Exception
{

}