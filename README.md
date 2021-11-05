# MediatR proxy interface generator

This generator generates interface and interface implementation for you based on your [MediatR](https://github.com/jbogard/MediatR) requests.


## Installation

```bash
  dotnet add package SourceGenerator.MediatR.Proxy
```

## How to use it

Define assembly info in contract/shared assembly.
```csharp
[assembly: MediatrProxyContract("IMyService", "Project.Shared")]
```

Define assembly info in project where implementation should be.
```csharp
[assembly: MediatrProxy("IMyService", "Project.Shared", "Project.Application.Service")]
```

TODO: Generates ... see tests for examples.

```csharp
public interface IMyService { .. }

public class MyService : IMyService { ..}
```

## Customize

...