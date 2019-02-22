# CommandQuery.AspNet.WebApi

> Command Query Separation for ASP.NET Web API 2 🌐

* Provides generic actions for handling the execution of commands and queries
* Enables APIs based on HTTP `POST` and `GET`

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.AspNet.WebApi.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AspNet.WebApi.svg)](https://www.nuget.org/packages/CommandQuery.AspNet.WebApi)

`PM>` `Install-Package CommandQuery.AspNet.WebApi`

`>` `dotnet add package CommandQuery.AspNet.WebApi`

## Sample Code

[`CommandQuery.Sample.AspNet.WebApi`](/samples/CommandQuery.Sample.AspNet.WebApi)

[`CommandQuery.Sample.AspNet.WebApi.Tests`](/samples/CommandQuery.Sample.AspNet.WebApi.Tests)

## Get Started

1. Create a new **ASP.NET Web API 2** project
	* [Tutorial](https://docs.microsoft.com/en-us/aspnet/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)
2. Install the `CommandQuery.AspNet.WebApi` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AspNet.WebApi)
	* `PM>` `Install-Package CommandQuery.AspNet.WebApi`
3. Create controllers
	* Inherit from `BaseCommandController` and `BaseQueryController`
4. Create commands and command handlers
	* Implement `ICommand` and `ICommandHandler<in TCommand>`
5. Create queries and query handlers
	* Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`
6. Configure dependency injection

## Commands

Add a `CommandController`:

```csharp
using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Sample.AspNet.WebApi.Controllers
{
    [RoutePrefix("api/command")]
    public class CommandController : BaseCommandController
    {
        public CommandController(ICommandProcessor commandProcessor, ITraceWriter logger) : base(commandProcessor, logger)
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
public async Task<IHttpActionResult> Handle(string commandName, [FromBody] Newtonsoft.Json.Linq.JObject json)
```

* The action is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the command is the slug of the URL.
* The command itself is provided as JSON in the body.
* If the command succeeds; the response is empty with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Example of a command request via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Value':'sv-SE'}" http://localhost:55359/api/command/FooCommand --header "Content-Type:application/json"`

## Queries

Add a `QueryController`:

```csharp
using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Sample.AspNet.WebApi.Controllers
{
    [RoutePrefix("api/query")]
    public class QueryController : BaseQueryController
    {
        public QueryController(IQueryProcessor queryProcessor, ITraceWriter logger) : base(queryProcessor, logger)
        {
        }
    }
}
```

Inherit from `BaseQueryController` and pass the `IQueryProcessor` to the base constructor.

The action methods from the base class will handle all queries:

```csharp
[HttpPost]
[Route("{queryName}")]
public async Task<IHttpActionResult> HandlePost(string queryName, [FromBody] Newtonsoft.Json.Linq.JObject json)
```

```csharp
[HttpGet]
[Route("{queryName}")]
public async Task<IHttpActionResult> HandleGet(string queryName)
```

* The action is requested via:
    * HTTP `POST` with the Content-Type `application/json` in the header and the query itself as JSON in the body
    * HTTP `GET` and the query itself as query string parameters in the URL
* The name of the query is the slug of the URL.
* If the query succeeds; the response is the result as JSON with the HTTP status code `200`.
* If the query fails; the response is an error message with the HTTP status code `400` or `500`.

Example of query requests via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Id':1}" http://localhost:55359/api/query/BarQuery --header "Content-Type:application/json"`

`curl -X GET http://localhost:55359/api/query/BarQuery?Id=1`

## Configuration

Configuration in `App_Start/WebApiConfig.cs`

```csharp
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AspNet.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // IoC
            var services = new ServiceCollection();

            services.AddCommands(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
            services.AddQueries(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

            services.AddTransient<ICultureService, CultureService>();
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();

            services.AddTransient<ITraceWriter>(_ => config.EnableSystemDiagnosticsTracing()); // Logging

            config.DependencyResolver = new CommandQueryDependencyResolver(services);

            // Web API routes
            config.MapHttpAttributeRoutes(new CommandQueryDirectRouteProvider());

            // Json
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
        }
    }
}
```

Register command/query handlers and other dependencies in the `Register` method.

The extension methods `AddCommands` and `AddQueries` will add all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(WebApiConfig).Assembly` as a single argument.

To enable logging, inject a `ITraceWriter` to the constructor of the controllers.

The `CommandQueryDependencyResolver` will manage the dependency injection when controllers are created.

The `CommandQueryDirectRouteProvider` makes sure the routes from the base controllers are inherited.

Consider to use only the `JsonMediaTypeFormatter`.

## Testing

You can [unit test](https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api) your controllers and command/query handlers.

Test commands:

```csharp
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.Sample.AspNet.WebApi.Controllers;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNet.WebApi.Tests
{
    public class CommandControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            var configuration = new HttpConfiguration();
            WebApiConfig.Register(configuration);

            var commandProcessor = configuration.DependencyResolver.GetService(typeof(ICommandProcessor)) as ICommandProcessor;

            Subject = new CommandController(commandProcessor, null)
            {
                Request = new HttpRequestMessage(),
                Configuration = configuration
            };
        }

        [Test]
        public async Task should_work()
        {
            var json = JObject.Parse("{ 'Value': 'Foo' }");
            var result = await Subject.Handle("FooCommand", json) as OkResult;

            result.Should().NotBeNull();
        }

        CommandController Subject;
    }
}
```

Test queries:

```csharp
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using CommandQuery.Sample.AspNet.WebApi.Controllers;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNet.WebApi.Tests
{
    public class QueryControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            var configuration = new HttpConfiguration();
            WebApiConfig.Register(configuration);

            var queryProcessor = configuration.DependencyResolver.GetService(typeof(IQueryProcessor)) as IQueryProcessor;

            Subject = new QueryController(queryProcessor, null)
            {
                Request = new HttpRequestMessage(),
                Configuration = configuration
            };
        }

        [Test]
        public async Task should_work()
        {
            var json = JObject.Parse("{ 'Id': 1 }");
            var result = await (await Subject.HandlePost("BarQuery", json)).ExecuteAsync(CancellationToken.None);
            var value = await result.Content.ReadAsAsync<Bar>();

            result.EnsureSuccessStatusCode();
            value.Id.Should().Be(1);
            value.Value.Should().NotBeEmpty();
        }

        QueryController Subject;
    }
}
```
