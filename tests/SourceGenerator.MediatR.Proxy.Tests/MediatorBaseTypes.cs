using MediatR;
using System;

namespace SourceGenerator.MediatR.Proxy.Tests
{
    public abstract class RequestBase<TResponse> : IRequest<TResponse>
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString("D");
    }

    public abstract class Command<TResponse> : RequestBase<TResponse> { }

    public abstract class Query<TResponse> : RequestBase<TResponse> { }
}
