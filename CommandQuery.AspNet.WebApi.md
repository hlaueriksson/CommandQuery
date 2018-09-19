# CommandQuery.AspNet.WebApi

> Command Query Separation for ASP.NET Web API 2

* Provides generic actions for handling the execution of commands and queries
* Enables APIs based on HTTP `POST` and `GET`

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.AspNet.WebApi.svg?style=flat-square) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AspNet.WebApi.svg?style=flat-square)](https://www.nuget.org/packages/CommandQuery.AspNet.WebApi)

`PM>` `Install-Package CommandQuery.AspNet.WebApi`

`>` `dotnet add package CommandQuery.AspNet.WebApi`

## Sample Code

[`CommandQuery.Sample.AspNet.WebApi`](/sample/CommandQuery.Sample.AspNet.WebApi)

[`CommandQuery.Sample.AspNet.WebApi.Specs`](/sample/CommandQuery.Sample.AspNet.WebApi.Specs)

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
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Sample.AspNet.WebApi.Controllers
{
    [RoutePrefix("api/command")]
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
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Sample.AspNet.WebApi.Controllers
{
    [RoutePrefix("api/query")]
    public class QueryController : BaseQueryController
    {
        public QueryController(IQueryProcessor queryProcessor) : base(queryProcessor)
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
using CommandQuery.AspNet.WebApi;
using CommandQuery.Sample.Commands;
using CommandQuery.Sample.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AspNet.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // IoC
            var services = new ServiceCollection();

            services.AddControllers(typeof(WebApiConfig).Assembly);

            services.AddCommands(typeof(FooCommand).Assembly);
            services.AddQueries(typeof(BarQuery).Assembly);

            services.AddTransient<IDateTimeProxy, DateTimeProxy>();

            config.DependencyResolver = new CommandQueryDependencyResolver(services);

            // Json
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            // Web API routes
            config.MapHttpAttributeRoutes(new CommandQueryDirectRouteProvider());
        }
    }
}
```

Register command/query handlers and other dependencies in the `Register` method.

The extension methods `AddCommands` and `AddQueries` will add all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(WebApiConfig).Assembly` as a single argument.

## Testing

You can [unit test](https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api) your controllers and command/query handlers.

Test commands:

```csharp
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.Sample.AspNet.WebApi.Controllers;
using Machine.Specifications;
using Newtonsoft.Json.Linq;

namespace CommandQuery.Sample.AspNet.WebApi.Specs.Controllers
{
    public class CommandControllerSpecs
    {
        [Subject(typeof(CommandController))]
        public class when_using_the_real_controller
        {
            Establish context = () =>
            {
                var configuration = new HttpConfiguration();
                WebApiConfig.Register(configuration);

                var commandProcessor = configuration.DependencyResolver.GetService(typeof(ICommandProcessor)) as ICommandProcessor;

                Subject = new CommandController(commandProcessor)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = configuration
                };
            };

            It should_work = () =>
            {
                var json = JObject.Parse("{ 'Value': 'Foo' }");
                var result = Subject.Handle("FooCommand", json).Result as OkResult;

                result.ShouldNotBeNull();
            };

            It should_handle_errors = () =>
            {
                var json = JObject.Parse("{ 'Value': 'Foo' }");
                var result = Subject.Handle("FailCommand", json).Result;

                result.ShouldBeError("The command type 'FailCommand' could not be found");
            };

            static CommandController Subject;
        }
    }
}
```

Test queries:

```csharp
using System;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using CommandQuery.Sample.AspNet.WebApi.Controllers;
using CommandQuery.Sample.Queries;
using Machine.Specifications;
using Newtonsoft.Json.Linq;

namespace CommandQuery.Sample.AspNet.WebApi.Specs.Controllers
{
    public class QueryControllerSpecs
    {
        [Subject(typeof(QueryController))]
        public class when_using_the_real_controller
        {
            Establish context = () =>
            {
                var configuration = new HttpConfiguration();
                WebApiConfig.Register(configuration);

                var queryProcessor = configuration.DependencyResolver.GetService(typeof(IQueryProcessor)) as IQueryProcessor;

                Subject = new QueryController(queryProcessor)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = configuration
                };
            };

            public class method_Post
            {
                It should_work = () =>
                {
                    var json = JObject.Parse("{ 'Id': 1 }");
                    var result = Subject.HandlePost("BarQuery", json).Result.ExecuteAsync(CancellationToken.None).Result;
                    var value = result.Content.ReadAsAsync<Bar>().Result;

                    result.EnsureSuccessStatusCode();
                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var json = JObject.Parse("{ 'Id': 1 }");
                    var result = Subject.HandlePost("FailQuery", json).Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            public class method_Get
            {
                Establish context = () =>
                {
                    Subject.Request = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://localhost/api/query/BarQuery?Id=1")
                    };
                };

                It should_work = () =>
                {
                    var result = Subject.HandleGet("BarQuery").Result.ExecuteAsync(CancellationToken.None).Result;
                    var value = result.Content.ReadAsAsync<Bar>().Result;

                    result.EnsureSuccessStatusCode();
                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var result = Subject.HandleGet("FailQuery").Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            static QueryController Subject;
        }
    }
}
```

Helpers:

```csharp
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Machine.Specifications;

namespace CommandQuery.Sample.AspNet.WebApi.Specs.Controllers
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this IHttpActionResult result, string message)
        {
            result.ExecuteAsync(CancellationToken.None).Result.ShouldBeError(message);
        }

        public static void ShouldBeError(this HttpResponseMessage result, string message)
        {
            result.ShouldNotBeNull();
            result.IsSuccessStatusCode.ShouldBeFalse();
            var value = result.Content.ReadAsStringAsync().Result;
            value.ShouldNotBeNull();
            value.ShouldContain(message);
        }
    }
}
```