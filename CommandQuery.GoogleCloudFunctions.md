# CommandQuery.GoogleCloudFunctions ⚡

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml) [![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

> Command Query Separation for Google Cloud Functions

* Provides generic function support for commands and queries with *HTTP functions*
* Enables APIs based on HTTP `POST` and `GET`

## Get Started

0. Install **Google.Cloud.Functions.Templates**
   * [https://www.nuget.org/packages/Google.Cloud.Functions.Templates/](https://www.nuget.org/packages/Google.Cloud.Functions.Templates/)
1. Create a new **gcf-http** project
   * [Tutorial](https://github.com/GoogleCloudPlatform/functions-framework-dotnet#quickstarts)
2. Install the `CommandQuery.GoogleCloudFunctions` package from [NuGet](https://www.nuget.org/packages/CommandQuery.GoogleCloudFunctions)
   * `PM>` `Install-Package CommandQuery.GoogleCloudFunctions`
3. Create functions
   * Preferably named `Command` and `Query`
4. Create commands and command handlers
   * Implement `ICommand` and `ICommandHandler<in TCommand>`
   * Or `ICommand<TResult>` and `ICommandHandler<in TCommand, TResult>`
5. Create queries and query handlers
   * Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`
6. Configure services in `Startup.cs`

## Commands

```cs
using CommandQuery.GoogleCloudFunctions;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    [FunctionsStartup(typeof(Startup))]
    public class Command(ICommandFunction commandFunction) : IHttpFunction
    {
        public async Task HandleAsync(HttpContext context)
        {
            var commandName = context.Request.Path.Value!.Substring("/api/command/".Length);

            await commandFunction.HandleAsync(commandName, context, null, context.RequestAborted);
        }
    }
}
```

* The function is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the command is the slug of the URL.
* The command itself is provided as JSON in the body.
* If the command succeeds; the response is empty with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Commands with result:

* If the command succeeds; the response is the result as JSON with the HTTP status code `200`.

## Queries

```cs
using CommandQuery.GoogleCloudFunctions;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    [FunctionsStartup(typeof(Startup))]
    public class Query(IQueryFunction queryFunction) : IHttpFunction
    {
        public async Task HandleAsync(HttpContext context)
        {
            var queryName = context.Request.Path.Value!.Substring("/api/query/".Length);

            await queryFunction.HandleAsync(queryName, context, null, context.RequestAborted);
        }
    }
}
```

* The function is requested via:
  * HTTP `POST` with the Content-Type `application/json` in the header and the query itself as JSON in the body
  * HTTP `GET` and the query itself as query string parameters in the URL
* The name of the query is the slug of the URL.
* If the query succeeds; the response is the result as JSON with the HTTP status code `200`.
* If the query fails; the response is an error message with the HTTP status code `400` or `500`.

## Configuration

Configuration in `Startup.cs`:

```cs
using CommandQuery.GoogleCloudFunctions;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
            services
                //.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web))

                // Add commands and queries
                .AddCommandFunction(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly)
                .AddQueryFunction(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly)

                // Add handler dependencies
                .AddTransient<IDateTimeProxy, DateTimeProxy>()
                .AddTransient<ICultureService, CultureService>();

        public override void Configure(WebHostBuilderContext context, IApplicationBuilder app)
        {
            // Validation
            app.ApplicationServices.GetService<ICommandProcessor>()!.AssertConfigurationIsValid();
            app.ApplicationServices.GetService<IQueryProcessor>()!.AssertConfigurationIsValid();
        }
    }
}
```

The extension methods `AddCommandFunction` and `AddQueryFunction` will add functions and all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(Startup).Assembly` as a single argument.

## Testing

You can [integration test](https://github.com/GoogleCloudPlatform/functions-framework-dotnet/blob/main/docs/testing.md) your functions with the [Google.Cloud.Functions.Testing](https://www.nuget.org/packages/Google.Cloud.Functions.Testing) package.

```cs
using System.Net.Http.Json;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Google.Cloud.Functions.Testing;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace CommandQuery.Sample.GoogleCloudFunctions.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                Server = new FunctionTestServer<Query>();
                Client = Server.CreateClient();
            }

            [TearDown]
            public void TearDown()
            {
                Client.Dispose();
                Server.Dispose();
            }

            [Test]
            public async Task should_work()
            {
                var response = await Client.PostAsJsonAsync("/api/query/BarQuery", new BarQuery { Id = 1 });
                var value = await response.Content.ReadFromJsonAsync<Bar>();
                value!.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var response = await Client.PostAsJsonAsync("/api/query/FailQuery", new BarQuery { Id = 1 });
                await response.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            FunctionTestServer<Query> Server = null!;
            HttpClient Client = null!;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                Server = new FunctionTestServer<Query>();
                Client = Server.CreateClient();
            }

            [TearDown]
            public void TearDown()
            {
                Client.Dispose();
                Server.Dispose();
            }

            [Test]
            public async Task should_work()
            {
                var response = await Client.GetAsync("/api/query/BarQuery?Id=1");
                var value = await response.Content.ReadFromJsonAsync<Bar>();
                value!.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var response = await Client.GetAsync("/api/query/FailQuery?Id=1");
                await response.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            FunctionTestServer<Query> Server = null!;
            HttpClient Client = null!;
        }
    }
}
```

## Samples

* [CommandQuery.Sample.GoogleCloudFunctions](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.GoogleCloudFunctions)
* [CommandQuery.Sample.GoogleCloudFunctions.Tests](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.GoogleCloudFunctions.Tests)
