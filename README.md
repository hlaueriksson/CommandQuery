# CommandQuery<!-- omit in toc -->

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml)
[![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

[![CommandQuery](https://img.shields.io/nuget/v/CommandQuery.svg?label=CommandQuery)](https://www.nuget.org/packages/CommandQuery)
[![CommandQuery.Abstractions](https://img.shields.io/nuget/v/CommandQuery.Abstractions.svg?label=CommandQuery.Abstractions)](https://www.nuget.org/packages/CommandQuery.Abstractions)

[![CommandQuery.AspNetCore](https://img.shields.io/nuget/v/CommandQuery.AspNetCore.svg?label=CommandQuery.AspNetCore)](https://www.nuget.org/packages/CommandQuery.AspNetCore)
[![CommandQuery.AWSLambda](https://img.shields.io/nuget/v/CommandQuery.AWSLambda.svg?label=CommandQuery.AWSLambda)](https://www.nuget.org/packages/CommandQuery.AWSLambda)
[![CommandQuery.AzureFunctions](https://img.shields.io/nuget/v/CommandQuery.AzureFunctions.svg?label=CommandQuery.AzureFunctions)](https://www.nuget.org/packages/CommandQuery.AzureFunctions)
[![CommandQuery.GoogleCloudFunctions](https://img.shields.io/nuget/v/CommandQuery.GoogleCloudFunctions.svg?label=CommandQuery.GoogleCloudFunctions)](https://www.nuget.org/packages/CommandQuery.GoogleCloudFunctions)

[![CommandQuery.Client](https://img.shields.io/nuget/v/CommandQuery.Client.svg?label=CommandQuery.Client)](https://www.nuget.org/packages/CommandQuery.Client)

## Content<!-- omit in toc -->

- [Introduction](#introduction)
- [Packages](#packages)
  - [`CommandQuery` ⚙️](#commandquery-️)
  - [`CommandQuery.AspNetCore` 🌐](#commandqueryaspnetcore-)
  - [`CommandQuery.AWSLambda` ⚡](#commandqueryawslambda-)
  - [`CommandQuery.AzureFunctions` ⚡](#commandqueryazurefunctions-)
  - [`CommandQuery.GoogleCloudFunctions` ⚡](#commandquerygooglecloudfunctions-)
  - [`CommandQuery.Client` 🧰](#commandqueryclient-)
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
  - [`CommandQuery.Sample.AspNetCore`](/samples/CommandQuery.Sample.AspNetCore)
  - [`CommandQuery.Sample.AspNetCore.Tests`](/samples/CommandQuery.Sample.AspNetCore.Tests)

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
  - [`CommandQuery.Sample.AzureFunctions`](/samples/CommandQuery.Sample.AzureFunctions)
  - [`CommandQuery.Sample.AzureFunctions.Tests`](/samples/CommandQuery.Sample.AzureFunctions.Tests)

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

## Upgrading

> ⬆️ Upgrading from version `3.0.0` to `4.0.0`

Upgrade AspNetCore:

- Upgrade the project target framework to `net8.0`

Upgrade AWSLambda:

- Upgrade the project target framework to `net8.0`

Upgrade AzureFunctions:

- Upgrade the project target framework to `net8.0`
- Remove the `logger` argument from `HandleAsync`
- Consider to use the `HttpRequest` versions of `HandleAsync`

Upgrade GoogleCloudFunctions:

- Upgrade the project target framework to `net8.0`
- Remove the `logger` argument from `HandleAsync`

## Acknowledgements

Inspired by _Steven van Deursen_ blog posts:

- [Meanwhile... on the command side of my architecture](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-command-side-of-my-architecture/)
- [Meanwhile... on the query side of my architecture](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-query-side-of-my-architecture/)
