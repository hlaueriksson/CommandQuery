# CommandQuery

[![Build Status](https://img.shields.io/appveyor/ci/hlaueriksson/commandquery.svg?style=flat-square)](https://ci.appveyor.com/project/hlaueriksson/commandquery)
[![NuGet](https://img.shields.io/nuget/v/CommandQuery.svg?style=flat-square)](https://www.nuget.org/packages/CommandQuery)

## Introduction

Command Query Separation (CQS) for ASP.NET Core and Azure Functions

* Build services and functions that separate the responsibility of commands and queries
* Focus on implementing the handlers for commands and queries
* Make use of ASP.NET Core and Azure Functions to create APIs with CQS

Commands and Queries:
* [Commands](#commands)
* [Queries](#queries)

ASP.NET Core:
* [CommandQuery.AspNetCore](#commandqueryaspnetcore)

Azure Functions:
* [CommandQuery.AzureFunctions](#commandqueryazurefunctions)

Testing:
* [CommandQuery.AspNetCore](#commandqueryaspnetcore-1)
* [CommandQuery.AzureFunctions](#commandqueryazurefunctions-1)
* [Manual Testing](#manual-testing)

Inspired by:
* https://cuttingedge.it/blogs/steven/pivot/entry.php?id=91
* https://cuttingedge.it/blogs/steven/pivot/entry.php?id=92

[Command Query Separation](http://martinfowler.com/bliki/CommandQuerySeparation.html) in a nutshell:

* Commands
	* Writes (Create, Update, Delete) data
* Queries
	* Reads and returns data

## Commands

> Commands: Change the state of a system but do not return a value.
>
>  - [Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)

Download from NuGet: https://www.nuget.org/packages/CommandQuery/

Example code: [`CommandQuery.Sample`](/sample/CommandQuery.Sample)

Create a `Command` and `CommandHandler`:

```csharp
using System.Threading.Tasks;

namespace CommandQuery.Sample.Commands
{
    public class FooCommand : ICommand
    {
        public string Value { get; set; }
    }

    public class FooCommandHandler : ICommandHandler<FooCommand>
    {
        public async Task HandleAsync(FooCommand command)
        {
            // TODO: do some real command stuff

            await Task.Delay(10);
        }
    }
}
```

Commands implements the marker interface `ICommand` and command handlers implements `ICommandHandler<in TCommand>`.

## Queries

> Queries: Return a result and do not change the observable state of the system (are free of side effects).
>
>  - [Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)

Download from NuGet: https://www.nuget.org/packages/CommandQuery/

Example code: [`CommandQuery.Sample`](/sample/CommandQuery.Sample)

Create a `Query`, `QueryHandler` and `Result`:

```csharp
using System;
using System.Threading.Tasks;

namespace CommandQuery.Sample.Queries
{
    public class Bar
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }

    public class BarQuery : IQuery<Bar>
    {
        public int Id { get; set; }
    }

    public class BarQueryHandler : IQueryHandler<BarQuery, Bar>
    {
        public async Task<Bar> HandleAsync(BarQuery query)
        {
            var result = new Bar { Id = query.Id, Value = DateTime.Now.ToString("F") }; // TODO: do some real query stuff

            return await Task.FromResult(result);
        }
    }
}
```

Queries implements the marker interface `IQuery<TResult>` and query handlers implements `IQueryHandler<in TQuery, TResult>`.

## CommandQuery.AspNetCore

* Provides generic actions for handling the execution of commands and queries
* Provides an API based on HTTP `POST`

Download from NuGet: https://www.nuget.org/packages/CommandQuery.AspNetCore/

Example code: [`CommandQuery.Sample.AspNetCore`](/sample/CommandQuery.Sample.AspNetCore)

### Get Started

1. Create a new **ASP.NET Core 2.0** project
	* [Tutorials](https://docs.microsoft.com/en-us/aspnet/core/tutorials/)
2. Install the `CommandQuery.AspNetCore` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AspNetCore/)
	* `PM>` `Install-Package CommandQuery.AspNetCore`
3. Create controllers
	* Inherit from `BaseCommandController` and `BaseQueryController`
4. Create commands and command handlers
	* Implement `ICommand` and `ICommandHandler<in TCommand>`
5. Create queries and query handlers
	* Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`
6. Add the handlers to the dependency injection container
	* `services.AddCommands(typeof(Startup).Assembly);`
	* `services.AddQueries(typeof(Startup).Assembly);`

### Commands

Add a `CommandController`:

```csharp
using CommandQuery.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Sample.AspNetCore.Controllers
{
    [Route("api/[controller]")]
    public class CommandController : BaseCommandController
    {
        public CommandController(ICommandProcessor commandProcessor) : base(commandProcessor)
        {
        }
    }
}
```

Inherit from `BaseCommandController` and pass the `ICommandProcessor` to the base constructor.

The action method from the base class will handle all commands:

```csharp
[HttpPost]
[Route("{commandName}")]
public async Task<object> Handle(string commandName, [FromBody] Newtonsoft.Json.Linq.JObject json)
```

* The action is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the command is the slug of the URL.
* The command itself is provided as JSON in the body.
* If the command succeeds; the response is empty with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Example of a command request via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Value':'Foo'}" http://localhost:57857/api/command/FooCommand --header "Content-Type:application/json"`

Configure services in `Startup.cs`

```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc();

    // Add commands.
    services.AddCommands(typeof(Startup).Assembly);
}
```

The extension method `AddCommands` will add all command handlers in the given assemblies to the dependency injection container.
You can pass in a `params` array of `Assembly` arguments if your command handlers are located in different projects.
If you only have one project you can use `typeof(Startup).Assembly` as a single argument.

### Queries

Add a `QueryController`:

```csharp
using CommandQuery.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Sample.AspNetCore.Controllers
{
    [Route("api/[controller]")]
    public class QueryController : BaseQueryController
    {
        public QueryController(IQueryProcessor queryProcessor) : base(queryProcessor)
        {
        }
    }
}
```

Inherit from `BaseQueryController` and pass the `IQueryProcessor` to the base constructor.

The action method from the base class will handle all queries:

```csharp
[HttpPost]
[Route("{queryName}")]
public async Task<object> Handle(string queryName, [FromBody] Newtonsoft.Json.Linq.JObject json)
```

* The action is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the query is the slug of the URL.
* The query itself is provided as JSON in the body.
* If the query succeeds; the response is the result as JSON with the HTTP status code `200`.
* If the query fails; the response is an error message with the HTTP status code `400` or `500`.

Example of a query request via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Id':1}" http://localhost:57857/api/query/BarQuery --header "Content-Type:application/json"`

Configure services in `Startup.cs`

```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc();

    // Add queries.
    services.AddQueries(typeof(Startup).Assembly);
}
```

The extension method `AddQueries` will add all query handlers in the given assemblies to the dependency injection container.
You can pass in a `params` array of `Assembly` arguments if your query handlers are located in different projects.
If you only have one project you can use `typeof(Startup).Assembly` as a single argument.

## CommandQuery.AzureFunctions

* Provides generic function support for commands and queries with **HTTPTriggers**
* Enables APIs based on HTTP `POST`

Download from NuGet: https://www.nuget.org/packages/CommandQuery.AzureFunctions/

Example code in **Visual Studio**:

* [`CommandQuery.Sample.AzureFunctions.Vs1`](/sample/CommandQuery.Sample.AzureFunctions.Vs1) - Azure Functions v1 (.NET Framework)
* [`CommandQuery.Sample.AzureFunctions.Vs2`](/sample/CommandQuery.Sample.AzureFunctions.Vs2) - Azure Functions v2 (.NET Core)

Example code in **Visual Studio Code**:

* [`CommandQuery.Sample.AzureFunctions.VsCode1`](/sample/CommandQuery.Sample.AzureFunctions.VsCode1) - Azure Functions v1 (.NET Framework)
* [`CommandQuery.Sample.AzureFunctions.VsCode2`](/sample/CommandQuery.Sample.AzureFunctions.VsCode2) - Azure Functions v2 (.NET Core)

### Get Started in **Visual Studio**

1. Create a new **Azure Functions** project
	* [Tutorial](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio)
2. Install the `CommandQuery.AzureFunctions` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AzureFunctions/)
	* `PM>` `Install-Package CommandQuery.AzureFunctions`
3. Create functions
	* For example named `Command` and `Query`
4. Create commands and command handlers
	* Implement `ICommand` and `ICommandHandler<in TCommand>`
5. Create queries and query handlers
	* Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`

![Add New Project - Azure Functions](vs-new-project-azure-functions.png)

When you create a new project in Visual Studio you need to choose the runtime:

* Azure Functions v1 (.NET Framework)
* Azure Functions v2 (.NET Core)

![Azure Functions v1 (.NET Framework)](vs-azure-functions-v1.png)
![Azure Functions v2 (.NET Core)](vs-azure-functions-v2.png)

### Get Started in **Visual Studio Code**

1. Create a new **Azure Functions** project
	* [Tutorial](https://code.visualstudio.com/tutorials/functions-extension/getting-started)
2. Install the `CommandQuery.AzureFunctions` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AzureFunctions/)
	* Edit the `.csproj` file and add `<PackageReference Include="CommandQuery.AzureFunctions" Version="0.3.2" />`
3. Create functions
	* For example named `Command` and `Query`
4. Create commands and command handlers
	* Implement `ICommand` and `ICommandHandler<in TCommand>`
5. Create queries and query handlers
	* Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`

![Azure Functions for Visual Studio Code](vscode-azure-functions.png)

Before you create a new project in Visual Studio Code you need to install the runtime:

* Azure Functions v1 (.NET Framework)
    * `npm i -g azure-functions-core-tools`
* Azure Functions v2 (.NET Core)
    * `npm i -g azure-functions-core-tools@core --unsafe-perm true`

And install the _.NET Templates for Azure Functions_:

![Azure Functions: Install templates for the .NET CLI](vscode-azure-functions-templates-1.png)
![Select the template version to install](vscode-azure-functions-templates-2.png)

### Commands

Add a `Command` function in *Azure Functions v1 (.NET Framework)*:

```csharp
using System.Net.Http;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Commands;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.AzureFunctions.Vs1
{
    public static class Command
    {
        private static readonly CommandFunction Func = new CommandFunction(typeof(FooCommand).Assembly.GetCommandProcessor());

        [FunctionName("Command")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequestMessage req, TraceWriter log, string commandName)
        {
            return await Func.Handle(commandName, req, log);
        }
    }
}
```

Add a `Command` function in *Azure Functions v2 (.NET Core)*:

```csharp
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.AzureFunctions.Vs2
{
    public static class Command
    {
        private static readonly CommandFunction Func = new CommandFunction(typeof(FooCommand).Assembly.GetCommandProcessor());

        [FunctionName("Command")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequest req, TraceWriter log, string commandName)
        {
            return await Func.Handle(commandName, req, log);
        }
    }
}
```

* The function is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the command is the slug of the URL.
* The command itself is provided as JSON in the body.
* If the command succeeds; the response is empty with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Example of a command request via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Value':'Foo'}" http://localhost:7071/api/command/FooCommand --header "Content-Type:application/json"`

### Queries

Add a `Query` function in *Azure Functions v1 (.NET Framework)*:

```csharp
using System.Net.Http;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Queries;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.AzureFunctions.Vs1
{
    public static class Query
    {
        private static readonly QueryFunction Func = new QueryFunction(typeof(BarQuery).Assembly.GetQueryProcessor());

        [FunctionName("Query")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "query/{queryName}")] HttpRequestMessage req, TraceWriter log, string queryName)
        {
            return await Func.Handle(queryName, req, log);
        }
    }
}
```

Add a `Query` function in *Azure Functions v2 (.NET Core)*:

```csharp
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.AzureFunctions.Vs2
{
    public static class Query
    {
        private static readonly QueryFunction Func = new QueryFunction(typeof(BarQuery).Assembly.GetQueryProcessor());

        [FunctionName("Query")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "query/{queryName}")] HttpRequest req, TraceWriter log, string queryName)
        {
            return await Func.Handle(queryName, req, log);
        }
    }
}
```

* The function is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the query is the slug of the URL.
* The query itself is provided as JSON in the body.
* If the query succeeds; the response is the result as JSON with the HTTP status code `200`.
* If the query fails; the response is an error message with the HTTP status code `400` or `500`.

Example of a query request via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Id':1}" http://localhost:7071/api/query/BarQuery --header "Content-Type:application/json"`

## Testing

### CommandQuery.AspNetCore

You can [integration test](https://docs.microsoft.com/en-us/aspnet/core/testing/integration-testing) your controllers and command/query handlers with the `Microsoft.AspNetCore.TestHost`.

Example code: [`CommandQuery.Sample.Specs`](/sample/CommandQuery.Sample.Specs).

Test commands:

```csharp
using System.Net.Http;
using System.Text;
using CommandQuery.Sample.AspNetCore;
using CommandQuery.Sample.AspNetCore.Controllers;
using Machine.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace CommandQuery.Sample.Specs.AspNetCore.Controllers
{
    public class CommandControllerSpecs
    {
        [Subject(typeof(CommandController))]
        public class when_using_the_real_controller
        {
            Establish context = () =>
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            };

            It should_work = () =>
            {
                var content = new StringContent("{ 'Value': 'Foo' }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/command/FooCommand", content).Result;

                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsStringAsync().Result;

                result.ShouldBeEmpty();
            };

            It should_handle_errors = () =>
            {
                var content = new StringContent("{ 'Value': 'Foo' }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/command/FailCommand", content).Result;

                response.IsSuccessStatusCode.ShouldBeFalse();

                var result = response.Content.ReadAsStringAsync().Result;

                result.ShouldEqual("The command type 'FailCommand' could not be found");
            };

            static TestServer Server;
            static HttpClient Client;
        }
    }
}
```

Test queries:

```csharp
using System.Net.Http;
using System.Text;
using CommandQuery.Sample.Queries;
using CommandQuery.Sample.AspNetCore;
using CommandQuery.Sample.AspNetCore.Controllers;
using Machine.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace CommandQuery.Sample.Specs.AspNetCore.Controllers
{
    public class QueryControllerSpecs
    {
        [Subject(typeof(QueryController))]
        public class when_using_the_real_controller
        {
            Establish context = () =>
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            };

            It should_work = () =>
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/query/BarQuery", content).Result;

                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<Bar>().Result;

                result.Id.ShouldEqual(1);
                result.Value.ShouldNotBeEmpty();
            };

            It should_handle_errors = () =>
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/query/FailQuery", content).Result;

                response.IsSuccessStatusCode.ShouldBeFalse();

                var result = response.Content.ReadAsStringAsync().Result;

                result.ShouldEqual("The query type 'FailQuery' could not be found");
            };

            static TestServer Server;
            static HttpClient Client;
        }
    }
}
```

### CommandQuery.AzureFunctions

Example code: [`CommandQuery.Sample.Specs`](/sample/CommandQuery.Sample.Specs).

Fake log:

```csharp
using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs2
{
    public class FakeTraceWriter : TraceWriter
    {
        public FakeTraceWriter() : base(TraceLevel.Off)
        {
        }

        public override void Trace(TraceEvent traceEvent)
        {
        }
    }
}
```

Test commands:

```csharp
using System.IO;
using CommandQuery.Sample.AzureFunctions.Vs2;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs2
{
    public class CommandSpecs
    {
        [Subject(typeof(Command))]
        public class when_using_the_real_function
        {
            It should_work = () =>
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new FakeTraceWriter();

                var result = Command.Run(req, log, "FooCommand").Result as EmptyResult;

                result.ShouldNotBeNull();
            };

            It should_handle_errors = () =>
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new FakeTraceWriter();

                var result = Command.Run(req, log, "FailCommand").Result as BadRequestObjectResult;

                result.Value.ShouldEqual("The command type 'FailCommand' could not be found");
            };

            static DefaultHttpRequest GetHttpRequest(string content)
            {
                var httpContext = new DefaultHttpContext();
                httpContext.Features.Get<IHttpRequestFeature>().Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

                return new DefaultHttpRequest(httpContext);
            }
        }
    }
}
```

Test queries:

```csharp
using System.IO;
using CommandQuery.Sample.AzureFunctions.Vs2;
using CommandQuery.Sample.Queries;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs2
{
    public class QuerySpecs
    {
        [Subject(typeof(Query))]
        public class when_using_the_real_function
        {
            It should_work = () =>
            {
                var req = GetHttpRequest("{ 'Id': 1 }");
                var log = new FakeTraceWriter();

                var result = Query.Run(req, log, "BarQuery").Result as OkObjectResult;
                var value = result.Value as Bar;

                value.Id.ShouldEqual(1);
                value.Value.ShouldNotBeEmpty();
            };

            It should_handle_errors = () =>
            {
                var req = GetHttpRequest("{ 'Id': 1 }");
                var log = new FakeTraceWriter();

                var result = Query.Run(req, log, "FailQuery").Result as BadRequestObjectResult;

                result.Value.ShouldEqual("The query type 'FailQuery' could not be found");
            };

            static DefaultHttpRequest GetHttpRequest(string content)
            {
                var httpContext = new DefaultHttpContext();
                httpContext.Features.Get<IHttpRequestFeature>().Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

                return new DefaultHttpRequest(httpContext);
            }
        }
    }
}
```

### Manual Testing

1. Select your `AspNetCore` or `AzureFunctions` project and *Set as StartUp Project*
2. Run the project with `F5`
3. Manual test with **Postman**

You can import these collections in Postman to get started:

* [CommandQuery.Sample.AspNetCore.postman_collection.json](/sample/CommandQuery.Sample.AspNetCore.postman_collection.json)
* [CommandQuery.Sample.AzureFunctions.postman_collection.json](/sample/CommandQuery.Sample.AzureFunctions.postman_collection.json)

![Postman](postman.png)
