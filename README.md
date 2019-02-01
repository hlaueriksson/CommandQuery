# CommandQuery

[![Build Status](https://img.shields.io/appveyor/ci/hlaueriksson/commandquery.svg?style=flat-square)](https://ci.appveyor.com/project/hlaueriksson/commandquery)
[![NuGet](https://img.shields.io/nuget/v/CommandQuery.svg?style=flat-square)](https://www.nuget.org/packages/CommandQuery)

## Introduction

Command Query Separation (CQS) for .NET Framework and .NET Standard

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

Inspired by:
* https://cuttingedge.it/blogs/steven/pivot/entry.php?id=91
* https://cuttingedge.it/blogs/steven/pivot/entry.php?id=92

## Packages

README | Platform | NuGet | Sample
--- | --- | --- | ---
[CommandQuery](CommandQuery.md) | .NET Framework and .NET Standard | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.svg?style=flat-square) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.svg?style=flat-square)](https://www.nuget.org/packages/CommandQuery) | [`CommandQuery.Sample`](/samples/CommandQuery.Sample)
[CommandQuery.AspNet.WebApi](CommandQuery.AspNet.WebApi.md) | ASP.NET Web API 2 | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.AspNet.WebApi.svg?style=flat-square) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AspNet.WebApi.svg?style=flat-square)](https://www.nuget.org/packages/CommandQuery.AspNet.WebApi) | [`CommandQuery.Sample.AspNet.WebApi`](/samples/CommandQuery.Sample.AspNet.WebApi)
[CommandQuery.AspNetCore](CommandQuery.AspNetCore.md) | ASP.NET Core | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.AspNetCore.svg?style=flat-square) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AspNetCore.svg?style=flat-square)](https://www.nuget.org/packages/CommandQuery.AspNetCore) | [`CommandQuery.Sample.AspNetCore`](/samples/CommandQuery.Sample.AspNetCore)
[CommandQuery.AWSLambda](CommandQuery.AWSLambda.md) | AWS Lambda | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.AWSLambda.svg?style=flat-square) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AWSLambda.svg?style=flat-square)](https://www.nuget.org/packages/CommandQuery.AWSLambda) | [`CommandQuery.Sample.AWSLambda`](/samples/CommandQuery.Sample.AWSLambda)
[CommandQuery.AzureFunctions](CommandQuery.AzureFunctions.md) | Azure Functions | [![NuGet](https://img.shields.io/nuget/v/CommandQuery.AzureFunctions.svg?style=flat-square) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AzureFunctions.svg?style=flat-square)](https://www.nuget.org/packages/CommandQuery.AzureFunctions) | [`Vs1`](/samples/CommandQuery.Sample.AzureFunctions.Vs1), [`Vs2`](/samples/CommandQuery.Sample.AzureFunctions.Vs2), [`VsCode1`](/samples/CommandQuery.Sample.AzureFunctions.VsCode1), [`VsCode2`](/samples/CommandQuery.Sample.AzureFunctions.VsCode2)
