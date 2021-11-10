SourceGenerator.MediatR.Proxy
=

[![Version](https://img.shields.io/nuget/v/Sourcegenerator.MediatR.Proxy?color=royalblue)](https://www.nuget.org/packages/SourceGenerator.MediatR.Proxy)
[![Downloads](https://img.shields.io/nuget/dt/SourceGenerator.MediatR.Proxy?label=Nuget%20downloads)](https://www.nuget.org/packages/SourceGenerator.MediatR.Proxy)
[![License](https://img.shields.io/github/license/stiankroknes/sourcegenerator-mediatr-proxy)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/blob/main/LICENSE)
[![Build](https://img.shields.io/github/workflow/status/stiankroknes/sourcegenerator-mediatr-proxy/CI%20Build)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/actions)
[![GitHub forks](https://img.shields.io/github/forks/stiankroknes/sourcegenerator-mediatr-proxy)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/network/members)
[![GitHub stars](https://img.shields.io/github/stars/stiankroknes/sourcegenerator-mediatr-proxy)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/stargazers)

This generator generates interface and interface implementation for you based on your [MediatR](https://github.com/jbogard/MediatR) requests.

## Installation

```bash
  dotnet add package SourceGenerator.MediatR.Proxy
```

## How to use it

Define assembly level attribute in contract/shared assembly where interface should be generated.
```csharp
[assembly: MediatrProxyContract("IMyService", "Project.Shared")]
```

Define assembly level attribute in project where implementation should be generated.
```csharp
[assembly: MediatrProxyImplementation("IMyService", "Project.Shared", "Project.Application.Service")]
```

TODO: Generates ... see demo/tests for examples.

In contract assembly we have request.
```csharp
namespace MyApp.Shared
{
    public class GetSomeDataQuery : Query<SomeDataResult>
    {
    }

    public class SomeDataResult
    {
        public string? Data { get; set; }
    }
    
    public  class GenerateSomethingCommand : Command<SomethingResult>
    {
    }

    public class SomethingResult { }
}
```

Source generator will then create this interface in the contract/shared assembly.

_Note: Contract/shared need reference to System.ServiceModel.Primitives for now, will be configurable._

```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MyApp.Shared;

namespace MyApp.Shared
{
    [System.ServiceModel.ServiceContract]
    public interface IMyService
    {
        [System.ServiceModel.OperationContract]
        System.Threading.Tasks.Task<SomeDataResult> GetSomeData(MyApp.Shared.GetSomeDataQuery query, CancellationToken cancellationToken = default);
        [System.ServiceModel.OperationContract]
        System.Threading.Tasks.Task<SomethingResult> GenerateSomething(MyApp.Shared.GenerateSomethingCommand command, CancellationToken cancellationToken = default);
    }
}
```

And this implementation in the implementation assembly.
```csharp
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MyApp.Shared;

namespace MyApp.Application
{
    public class MyService : IMyService
    {
        private readonly IMediator mediator;
        public MyService(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public System.Threading.Tasks.Task<MyApp.Shared.SomeDataResult> GetSomeData(MyApp.Shared.GetSomeDataQuery query, CancellationToken cancellationToken = default) => mediator.Send(query, cancellationToken);
        public System.Threading.Tasks.Task<MyApp.Shared.SomethingResult> GenerateSomething(MyApp.Shared.GenerateSomethingCommand command, CancellationToken cancellationToken = default) => mediator.Send(command, cancellationToken);
    }
}
```

## Customize

...

