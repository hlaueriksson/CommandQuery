# CommandQuery

[![Build Status](https://img.shields.io/appveyor/ci/hlaueriksson/commandquery.svg?style=flat-square)](https://ci.appveyor.com/project/hlaueriksson/commandquery)
[![NuGet](https://img.shields.io/nuget/v/CommandQuery.svg?style=flat-square)](https://www.nuget.org/packages/CommandQuery)

## Introduction

Command Query Separation (CQS) for ASP.NET Core Microservices

* Build microservices that separate the responsibility of commands and queries.
* Focus on implementing the handlers for commands and queries, not on writing Web APIs.
* CommandQuery provides generic actions for handling the execution of all commands and queries.
* CommandQuery provides a API based on HTTP `POST`, not a ~~REST~~ API

Download from NuGet: https://www.nuget.org/packages/CommandQuery/

Inspired by:
* https://cuttingedge.it/blogs/steven/pivot/entry.php?id=91
* https://cuttingedge.it/blogs/steven/pivot/entry.php?id=92

For example code, take a look at the [`sample` folder](/sample).

## Commands

> Commands: Change the state of a system but do not return a value.
>
>  - [Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)

Add a `CommandController`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Sample.Controllers
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
* If the command succeeds; the response is an empty string with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Example of a command request via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Value':'Foo'}" http://localhost:57857/api/command/FooCommand --header "Content-Type:application/json"`

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

Configure services in `Startup.cs`

```csharp
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add commands.
            services.AddCommands(typeof(Startup).GetTypeInfo().Assembly);
        }
```

The extension method `AddCommands` will add all command handlers in the given assemblies to the dependency injection container.
You can pass in a `params` array of `Assembly` arguments if your command handlers are located in different projects.
If you only have one project you can use `typeof(Startup).GetTypeInfo().Assembly` as a single argument.

## Queries

> Queries: Return a result and do not change the observable state of the system (are free of side effects).
>
>  - [Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)

Add a `QueryController`:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Sample.Controllers
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

Configure services in `Startup.cs`

```csharp
		// This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add queries.
            services.AddQueries(typeof(Startup).GetTypeInfo().Assembly);
        }
```

The extension method `AddQueries` will add all query handlers in the given assemblies to the dependency injection container.
You can pass in a `params` array of `Assembly` arguments if your query handlers are located in different projects.
If you only have one project you can use `typeof(Startup).GetTypeInfo().Assembly` as a single argument.

## Testing

You can [integration test](https://docs.asp.net/en/latest/testing/integration-testing.html) your controllers and command/query handlers with the `Microsoft.AspNetCore.TestHost`.

For example code, take a look at the [`sample` folder](/sample/CommandQuery.Sample.Specs).

Test commands:

```csharp
using System.Net.Http;
using System.Text;
using CommandQuery.Sample.Controllers;
using Machine.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using It = Machine.Specifications.It;

namespace CommandQuery.Sample.Specs.Controllers
{
    public class CommandControllerSpecs
    {
        [Subject(typeof(CommandController))]
        public class when_using_the_real_API
        {
            Establish context = () =>
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            };

            It should_work = async () =>
            {
                var content = new StringContent("{ 'Value': 'Foo' }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/command/FooCommand", content).Result; // NOTE: await does not work

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                responseString.ShouldBeEmpty();
            };

            It should_handle_errors = async () =>
            {
                var content = new StringContent("{ 'Value': 'Foo' }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/command/FailCommand", content).Result; // NOTE: await does not work

                response.IsSuccessStatusCode.ShouldBeFalse();

                var responseString = await response.Content.ReadAsStringAsync();

                responseString.ShouldEqual("The command type 'FailCommand' could not be found");
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
using CommandQuery.Sample.Controllers;
using CommandQuery.Sample.Queries;
using Machine.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using It = Machine.Specifications.It;

namespace CommandQuery.Sample.Specs.Controllers
{
    public class QueryControllerSpecs
    {
        [Subject(typeof(QueryController))]
        public class when_using_the_real_API
        {
            Establish context = () =>
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            };

            It should_work = async () =>
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/query/BarQuery", content).Result; // NOTE: await does not work

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Bar>(responseString);

                result.Id.ShouldEqual(1);
                result.Value.ShouldNotBeEmpty();
            };

            It should_handle_errors = async () =>
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/query/FailQuery", content).Result; // NOTE: await does not work

                response.IsSuccessStatusCode.ShouldBeFalse();

                var responseString = await response.Content.ReadAsStringAsync();

                responseString.ShouldEqual("The query type 'FailQuery' could not be found");
            };

            static TestServer Server;
            static HttpClient Client;
        }
    }
}
```