# CommandQuery

[![Build status](https://ci.appveyor.com/api/projects/status/4fpuly9vx43r9oay?svg=true)](https://ci.appveyor.com/project/hlaueriksson/commandquery)
[![CodeFactor](https://www.codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://www.codefactor.io/repository/github/hlaueriksson/commandquery)

[![CommandQuery](https://img.shields.io/nuget/v/CommandQuery.svg?label=CommandQuery)](https://www.nuget.org/packages/CommandQuery)
[![CommandQuery.Abstractions](https://img.shields.io/nuget/v/CommandQuery.Abstractions.svg?label=CommandQuery.Abstractions)](https://www.nuget.org/packages/CommandQuery.Abstractions)
[![CommandQuery.AspNet.WebApi](https://img.shields.io/nuget/v/CommandQuery.AspNet.WebApi.svg?label=CommandQuery.AspNet.WebApi)](https://www.nuget.org/packages/CommandQuery.AspNet.WebApi)
[![CommandQuery.AspNetCore](https://img.shields.io/nuget/v/CommandQuery.AspNetCore.svg?label=CommandQuery.AspNetCore)](https://www.nuget.org/packages/CommandQuery.AspNetCore)
[![CommandQuery.AWSLambda](https://img.shields.io/nuget/v/CommandQuery.AWSLambda.svg?label=CommandQuery.AWSLambda)](https://www.nuget.org/packages/CommandQuery.AWSLambda)
[![CommandQuery.AzureFunctions](https://img.shields.io/nuget/v/CommandQuery.AzureFunctions.svg?label=CommandQuery.AzureFunctions)](https://www.nuget.org/packages/CommandQuery.AzureFunctions)
[![CommandQuery.Client](https://img.shields.io/nuget/v/CommandQuery.Client.svg?label=CommandQuery.Client)](https://www.nuget.org/packages/CommandQuery.Client)

## Introduction

Command Query Separation (CQS) for .NET Framework and .NET Core

* Build services that separate the responsibility of commands and queries
* Focus on implementing the handlers for commands and queries
* Create APIs with less boilerplate code

Available for:

```
🌐 ASP.NET Web API 2
🌐 ASP.NET Core
⚡ AWS Lambda
⚡ Azure Functions
```

Command Query Separation?

> **Queries**: Return a result and do not change the observable state of the system (are free of side effects).
>
> **Commands**: Change the state of a system but do not return a value.
>
> — <cite>[Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)</cite>

In other words:

* Commands
  * Writes (Create, Update, Delete) data
* Queries
  * Reads and returns data

The dogmatic approach to commands, that they *do not return a value*, can be inconvenient.
`CommandQuery` has a more pragmatic take and supports commands with result.

Inspired by:

* https://cuttingedge.it/blogs/steven/pivot/entry.php?id=91
* https://cuttingedge.it/blogs/steven/pivot/entry.php?id=92

## Packages

README | Platform | NuGet | Sample
--- | --- | --- | ---
[CommandQuery](CommandQuery.md) | .NET Framework and .NET Standard | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.svg)](https://www.nuget.org/packages/CommandQuery) | [`CommandQuery.Sample.Contracts`](/samples/CommandQuery.Sample.Contracts), [`CommandQuery.Sample.Handlers`](/samples/CommandQuery.Sample.Handlers)
[CommandQuery.AspNet.WebApi](CommandQuery.AspNet.WebApi.md) | ASP.NET Web API 2 | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.AspNet.WebApi.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AspNet.WebApi.svg)](https://www.nuget.org/packages/CommandQuery.AspNet.WebApi) | [`CommandQuery.Sample.AspNet.WebApi`](/samples/CommandQuery.Sample.AspNet.WebApi)
[CommandQuery.AspNetCore](CommandQuery.AspNetCore.md) | ASP.NET Core | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.AspNetCore.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AspNetCore.svg)](https://www.nuget.org/packages/CommandQuery.AspNetCore) | [`CommandQuery.Sample.AspNetCore.V3`](/samples/CommandQuery.Sample.AspNetCore.V3)
[CommandQuery.AWSLambda](CommandQuery.AWSLambda.md) | AWS Lambda | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.AWSLambda.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AWSLambda.svg)](https://www.nuget.org/packages/CommandQuery.AWSLambda) | [`CommandQuery.Sample.AWSLambda`](/samples/CommandQuery.Sample.AWSLambda)
[CommandQuery.AzureFunctions](CommandQuery.AzureFunctions.md) | Azure Functions | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.AzureFunctions.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AzureFunctions.svg)](https://www.nuget.org/packages/CommandQuery.AzureFunctions) | [`CommandQuery.Sample.AzureFunctions.Vs3`](/samples/CommandQuery.Sample.AzureFunctions.Vs3)
[CommandQuery.Client](CommandQuery.Client.md) | .NET Framework and .NET Standard | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.Client.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.Client.svg)](https://www.nuget.org/packages/CommandQuery.Client) | [`CommandQuery.Sample.Client`](/samples/CommandQuery.Sample.Client)
