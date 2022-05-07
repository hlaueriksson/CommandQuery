# CommandQuery.AspNet.WebApi 🌐

> Command Query Separation for ASP.NET Web API 2

* Provides generic actions for handling the execution of commands and queries
* Enables APIs based on HTTP `POST` and `GET`

## Get Started

1. Create a new **ASP.NET Web API 2** project
   * [Tutorial](https://docs.microsoft.com/en-us/aspnet/web-api/overview/getting-started-with-aspnet-web-api/tutorial-your-first-web-api)
2. Install the `CommandQuery.AspNet.WebApi` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AspNet.WebApi)
   * `PM>` `Install-Package CommandQuery.AspNet.WebApi`
3. Create controllers
   * Inherit from `BaseCommandController` and `BaseQueryController`
4. Create commands and command handlers
   * Implement `ICommand` and `ICommandHandler<in TCommand>`
   * Or `ICommand<TResult>` and `ICommandHandler<in TCommand, TResult>`
5. Create queries and query handlers
   * Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`
6. Configure dependency injection

## Commands

Add a `CommandController`:

```cs
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

```cs
/// <summary>
/// Handle a command.
/// </summary>
/// <param name="commandName">The name of the command</param>
/// <param name="json">The JSON representation of the command</param>
/// <returns>200, 400 or 500</returns>
[HttpPost]
[Route("{commandName}")]
public async Task<IHttpActionResult> Handle(string commandName, [FromBody] Newtonsoft.Json.Linq.JObject json)
```

* The action is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the command is the slug of the URL.
* The command itself is provided as JSON in the body.
* If the command succeeds; the response is empty with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Commands with result:

* If the command succeeds; the response is the result as JSON with the HTTP status code `200`.

## Queries

Add a `QueryController`:

```cs
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

```cs
/// <summary>
/// Handle a query.
/// </summary>
/// <param name="queryName">The name of the query</param>
/// <param name="json">The JSON representation of the query</param>
/// <returns>The result + 200, 400 or 500</returns>
[HttpPost]
[Route("{queryName}")]
public async Task<IHttpActionResult> HandlePost(string queryName, [FromBody] Newtonsoft.Json.Linq.JObject json)
```

```cs
/// <summary>
/// Handle a query.
/// </summary>
/// <param name="queryName">The name of the query</param>
/// <returns>The result + 200, 400 or 500</returns>
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

## Configuration

Configuration in `App_Start/WebApiConfig.cs`

```cs
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.AspNet.WebApi.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AspNet.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // IoC
            var services = new ServiceCollection();

            services.AddCommands(typeof(WebApiConfig).Assembly);
            services.AddQueries(typeof(WebApiConfig).Assembly);

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

To enable logging, inject an `ITraceWriter` to the constructor of the controllers.

The `CommandQueryDependencyResolver` will manage the dependency injection when controllers are created.

The `CommandQueryDirectRouteProvider` makes sure the routes from the base controllers are inherited.

Consider to clear the default formatters and only use the `JsonMediaTypeFormatter`.

## Testing

You can [unit test](https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api) your controllers and command/query handlers.

```cs
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using CommandQuery.Sample.AspNet.WebApi.Contracts.Queries;
using CommandQuery.Sample.AspNet.WebApi.Controllers;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNet.WebApi.Tests
{
    public class QueryControllerTests
    {
        public class when_using_the_real_controller_via_Post
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

            [Test]
            public async Task should_handle_errors()
            {
                var json = JObject.Parse("{ 'Id': 1 }");
                var result = await Subject.HandlePost("FailQuery", json);

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            QueryController Subject;
        }

        public class when_using_the_real_controller_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                var configuration = new HttpConfiguration();
                WebApiConfig.Register(configuration);

                var queryProcessor = configuration.DependencyResolver.GetService(typeof(IQueryProcessor)) as IQueryProcessor;

                Subject = new QueryController(queryProcessor, null)
                {
                    Request = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://localhost/api/query/BarQuery?Id=1")
                    },
                    Configuration = configuration
                };
            }

            [Test]
            public async Task should_work()
            {
                var result = await (await Subject.HandleGet("BarQuery")).ExecuteAsync(CancellationToken.None);
                var value = await result.Content.ReadAsAsync<Bar>();

                result.EnsureSuccessStatusCode();
                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Subject.HandleGet("FailQuery");

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            QueryController Subject;
        }
    }
}
```

## Samples

* [`CommandQuery.Sample.AspNet.WebApi`](/samples/CommandQuery.Sample.AspNet.WebApi)
* [`CommandQuery.Sample.AspNet.WebApi.Tests`](/samples/CommandQuery.Sample.AspNet.WebApi.Tests)
