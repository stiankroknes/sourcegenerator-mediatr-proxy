[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](/LICENSE)
[![GitHub issues](https://img.shields.io/github/issues/stiankroknes/sourcegenerator-mediatr-proxy)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/issues)
[![GitHub forks](https://img.shields.io/github/forks/stiankroknes/sourcegenerator-mediatr-proxy)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/network/members)
[![GitHub stars](https://img.shields.io/github/stars/stiankroknes/sourcegenerator-mediatr-proxy)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/stargazers)

[![NuGet Downloads (official NuGet)](https://img.shields.io/nuget/dt/SourceGenerator.MediatR.Proxy?label=NuGet%20Downloads)](https://www.nuget.org/packages/SourceGenerator.MediatR.Proxy/)

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
[assembly: MediatrProxyImplementation("IMyService", "Project.Shared", "Project.Application.Service")]
```

TODO: Generates ... see tests for examples.

```csharp
public interface IMyService { .. }

public class MyService : IMyService { ..}
```

## Customize

...
