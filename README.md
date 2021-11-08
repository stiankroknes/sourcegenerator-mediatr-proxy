SourceGenerator.MediatR.Proxy
=

[![Version](https://img.shields.io/nuget/v/Sourcegenerator.MediatR.Proxy?color=royalblue)](https://www.nuget.org/packages/SourceGenerator.MediatR.Proxy)
[![Downloads](https://img.shields.io/nuget/dt/SourceGenerator.MediatR.Proxy?label=Nuget%20downloads)](https://www.nuget.org/packages/SourceGenerator.MediatR.Proxy)
[![License](https://img.shields.io/github/license/stiankroknes/sourcegenerator-mediatr-proxy)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/blob/main/LICENSE)
[![Build](https://github.com/kzu/ThisAssembly/workflows/build/badge.svg?branch=main)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/actions)
[![GitHub forks](https://img.shields.io/github/forks/stiankroknes/sourcegenerator-mediatr-proxy)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/network/members)
[![GitHub stars](https://img.shields.io/github/stars/stiankroknes/sourcegenerator-mediatr-proxy)](https://github.com/stiankroknes/sourcegenerator-mediatr-proxy/stargazers)

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

TODO: Generates ... see demo/tests for examples.

```csharp
public interface IMyService { .. }

public class MyService : IMyService { ..}
```

## Customize

...
