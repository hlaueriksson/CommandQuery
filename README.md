# CommandQuery<!-- omit in toc -->

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml)
[![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

[![CommandQuery](https://img.shields.io/nuget/v/CommandQuery.svg?label=CommandQuery)](https://www.nuget.org/packages/CommandQuery)
[![CommandQuery.Abstractions](https://img.shields.io/nuget/v/CommandQuery.Abstractions.svg?label=CommandQuery.Abstractions)](https://www.nuget.org/packages/CommandQuery.Abstractions)
[![CommandQuery.Client](https://img.shields.io/nuget/v/CommandQuery.Client.svg?label=CommandQuery.Client)](https://www.nuget.org/packages/CommandQuery.Client)

[![CommandQuery.AspNetCore](https://img.shields.io/nuget/v/CommandQuery.AspNetCore.svg?label=CommandQuery.AspNetCore)](https://www.nuget.org/packages/CommandQuery.AspNetCore)
[![CommandQuery.AWSLambda](https://img.shields.io/nuget/v/CommandQuery.AWSLambda.svg?label=CommandQuery.AWSLambda)](https://www.nuget.org/packages/CommandQuery.AWSLambda)
[![CommandQuery.AzureFunctions](https://img.shields.io/nuget/v/CommandQuery.AzureFunctions.svg?label=CommandQuery.AzureFunctions)](https://www.nuget.org/packages/CommandQuery.AzureFunctions)
[![CommandQuery.GoogleCloudFunctions](https://img.shields.io/nuget/v/CommandQuery.GoogleCloudFunctions.svg?label=CommandQuery.GoogleCloudFunctions)](https://www.nuget.org/packages/CommandQuery.GoogleCloudFunctions)
[![CommandQuery.AspNet.WebApi](https://img.shields.io/nuget/v/CommandQuery.AspNet.WebApi.svg?label=CommandQuery.AspNet.WebApi)](https://www.nuget.org/packages/CommandQuery.AspNet.WebApi)

## Content<!-- omit in toc -->

- [Introduction](#introduction)
- [Packages](#packages)
  - [`CommandQuery` ⚙️](#commandquery-️)
  - [`CommandQuery.AspNetCore` 🌐](#commandqueryaspnetcore-)
  - [`CommandQuery.AWSLambda` ⚡](#commandqueryawslambda-)
  - [`CommandQuery.AzureFunctions` ⚡](#commandqueryazurefunctions-)
  - [`CommandQuery.GoogleCloudFunctions` ⚡](#commandquerygooglecloudfunctions-)
  - [`CommandQuery.Client` 🧰](#commandqueryclient-)
  - [`CommandQuery.AspNet.WebApi` 🌐](#commandqueryaspnetwebapi-)
- [Upgrading](#upgrading)
- [Acknowledgements](#acknowledgements)

## Introduction

Command Query Separation (CQS) for .NET and C#

- Build services that separate the responsibility of commands and queries
- Focus on implementing the handlers for commands and queries
- Create APIs with less boilerplate code

Available for:

```txt
🌐 ASP.NET Core
⚡ AWS Lambda
⚡ Azure Functions
⚡ Google Cloud Functions
🌐 ASP.NET Web API 2
```

Command Query Separation?

> **Queries**: Return a result and do not change the observable state of the system (are free of side effects).
>
> **Commands**: Change the state of a system but do not return a value.
>
> — <cite>[Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)</cite>

In other words:

- Commands
  - Writes (create, update, delete) data
- Queries
  - Reads and returns data

The traditional approach that commands *do not return a value* is a bit inconvenient.

`CommandQuery` has a pragmatic take and supports both commands with and without result 👍

## Packages

### `CommandQuery` ⚙️

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.svg)](https://www.nuget.org/packages/CommandQuery)

> Command Query Separation for .NET

- 📃 README: [CommandQuery.md](CommandQuery.md)
- 💁 Samples:
  - [`CommandQuery.Sample.Contracts`](/samples/CommandQuery.Sample.Contracts)
  - [`CommandQuery.Sample.Handlers`](/samples/CommandQuery.Sample.Handlers)

### `CommandQuery.AspNetCore` 🌐

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.AspNetCore.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AspNetCore.svg)](https://www.nuget.org/packages/CommandQuery.AspNetCore)

> Command Query Separation for ASP.NET Core

- 📃 README: [CommandQuery.AspNetCore.md](CommandQuery.AspNetCore.md)
- 💁 Samples:
  - [`CommandQuery.Sample.AspNetCore.V3`](/samples/CommandQuery.Sample.AspNetCore.V3)
  - [`CommandQuery.Sample.AspNetCore.V3.Tests`](/samples/CommandQuery.Sample.AspNetCore.V3.Tests)
  - [`CommandQuery.Sample.AspNetCore.V5`](/samples/CommandQuery.Sample.AspNetCore.V5)
  - [`CommandQuery.Sample.AspNetCore.V5.Tests`](/samples/CommandQuery.Sample.AspNetCore.V5.Tests)

### `CommandQuery.AWSLambda` ⚡

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.AWSLambda.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AWSLambda.svg)](https://www.nuget.org/packages/CommandQuery.AWSLambda)

> Command Query Separation for AWS Lambda

- 📃 README: [CommandQuery.AWSLambda.md](CommandQuery.AWSLambda.md)
- 💁 Samples:
  - [`CommandQuery.Sample.AWSLambda`](/samples/CommandQuery.Sample.AWSLambda)
  - [`CommandQuery.Sample.AWSLambda.Tests`](/samples/CommandQuery.Sample.AWSLambda.Tests)

### `CommandQuery.AzureFunctions` ⚡

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.AzureFunctions.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AzureFunctions.svg)](https://www.nuget.org/packages/CommandQuery.AzureFunctions)

> Command Query Separation for Azure Functions

- 📃 README: [CommandQuery.AzureFunctions.md](CommandQuery.AzureFunctions.md)
- 💁 Samples:
  - [`CommandQuery.Sample.AzureFunctions.V3`](/samples/CommandQuery.Sample.AzureFunctions.V3)
  - [`CommandQuery.Sample.AzureFunctions.V3.Tests`](/samples/CommandQuery.Sample.AzureFunctions.V3.Tests)
  - [`CommandQuery.Sample.AzureFunctions.V5`](/samples/CommandQuery.Sample.AzureFunctions.V5)
  - [`CommandQuery.Sample.AzureFunctions.V5.Tests`](/samples/CommandQuery.Sample.AzureFunctions.V5.Tests)

### `CommandQuery.GoogleCloudFunctions` ⚡

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.GoogleCloudFunctions.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.GoogleCloudFunctions.svg)](https://www.nuget.org/packages/CommandQuery.GoogleCloudFunctions)

> Command Query Separation for Google Cloud Functions

- 📃 README: [CommandQuery.GoogleCloudFunctions.md](CommandQuery.GoogleCloudFunctions.md)
- 💁 Samples:
  - [`CommandQuery.Sample.GoogleCloudFunctions`](/samples/CommandQuery.Sample.GoogleCloudFunctions)
  - [`CommandQuery.Sample.GoogleCloudFunctions.Tests`](/samples/CommandQuery.Sample.GoogleCloudFunctions.Tests)

### `CommandQuery.Client` 🧰

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.Client.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.Client.svg)](https://www.nuget.org/packages/CommandQuery.Client)

> Clients for CommandQuery APIs

- 📃 README: [CommandQuery.Client.md](CommandQuery.Client.md)
- 💁 Samples:
  - [`CommandQuery.Sample.Client`](/samples/CommandQuery.Sample.Client)

### `CommandQuery.AspNet.WebApi` 🌐

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.AspNet.WebApi.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AspNet.WebApi.svg)](https://www.nuget.org/packages/CommandQuery.AspNet.WebApi)

> Command Query Separation for ASP.NET Web API 2

⛔ This package is no longer maintained and new versions will not be published

- 📃 README: [CommandQuery.AspNet.WebApi.md](CommandQuery.AspNet.WebApi.md)
- 💁 Samples:
  - [`CommandQuery.Sample.AspNet.WebApi`](/samples/CommandQuery.Sample.AspNet.WebApi)
  - [`CommandQuery.Sample.AspNet.WebApi.Tests`](/samples/CommandQuery.Sample.AspNet.WebApi.Tests)

## Upgrading

> ⬆️ Upgrading from version `1.0.0` to `2.0.0`

Upgrade command/query handlers:

- Upgrade the project target framework from ~~`net461`~~ to `netstandard2.0` or greater
- Add a `CancellationToken` parameter to the `HandleAsync` methods in classes that implement `ICommandHandler<TCommand>`, `ICommandHandler<TCommand, TResult>` and `IQueryHandler<TQuery, TResult>`

Upgrade AspNet.WebApi:

- Migrate from `CommandQuery.AspNet.WebApi` to `CommandQuery.AspNetCore`

Upgrade AspNetCore:

- Consider to upgrade the project target framework to `netcoreapp3.1` or `net5.0`
- Consider to use the extension methods `AddCommandControllers` and `AddQueryControllers` in `Startup.cs`

Upgrade AWSLambda:

- Upgrade the project target framework to `netcoreapp3.1`
- Change the method invocation on `CommandFunction` and `QueryFunction` from ~~`Handle`~~ to `HandleAsync`
- Change the argument on `HandleAsync` methods from ~~`ILambdaContext`~~ to `ILambdaLogger`
- Consider to use the extension methods `AddCommandFunction` and `AddQueryFunction` on `IServiceCollection`
- Consider to use the `JsonSerializerOptions` constructor argument in `CommandFunction` and `QueryFunction` to configure JSON serialization/deserialization

Upgrade AzureFunctions:

- Upgrade the project target framework to `netcoreapp3.1` or `net5.0`
- Change the method invocation on `CommandFunction` and `QueryFunction` from ~~`Handle`~~ to `HandleAsync`
- Consider to use the extension methods `AddCommandControllers` and `AddQueryControllers` in `Startup.cs`/`Program.cs`
- Consider to use the `CancellationToken` argument on `HandleAsync` methods in `netcoreapp3.1` projects
- Consider to use the `JsonSerializerSettings`/`JsonSerializerOptions` constructor argument in `CommandFunction` and `QueryFunction` to configure JSON serialization/deserialization

Upgrade Client:

- Change the method invocation on `CommandClient` and `QueryClient` from ~~`Post`~~ to `PostAsync` and from ~~`Get`~~ to `GetAsync`
- Consider to use the `AddHttpClient` extension method on `IServiceCollection` to create the `CommandClient` and `QueryClient` (see [sample](https://github.com/hlaueriksson/CommandQuery/blob/master/samples/CommandQuery.Sample.Client/Program.cs))
- Consider to use the `CancellationToken` argument to methods in `CommandClient` and `QueryClient`

Validation:

- Consider to use the `AssertConfigurationIsValid` method on `CommandProcessor` and `QueryProcessor` to validate handler and type configuration

## Acknowledgements

Inspired by _Steven van Deursen_ blog posts:

- [Meanwhile... on the command side of my architecture](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-command-side-of-my-architecture/)
- [Meanwhile... on the query side of my architecture](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-query-side-of-my-architecture/)
