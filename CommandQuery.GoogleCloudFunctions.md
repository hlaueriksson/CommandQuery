### CommandQuery.GoogleCloudFunctions ⚡

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml) [![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

> Command Query Separation for Google Cloud Functions

* Provides generic function support for commands and queries with *HTTP functions*
* Enables APIs based on HTTP `POST` and `GET`

#### Get Started

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

#### Commands

```cs
using System.Threading.Tasks;
using CommandQuery.GoogleCloudFunctions;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    [FunctionsStartup(typeof(Startup))]
    public class Command : IHttpFunction
    {
        private readonly ILogger _logger;
        private readonly ICommandFunction _commandFunction;

        public Command(ILogger<Command> logger, ICommandFunction commandFunction)
        {
            _logger = logger;
            _commandFunction = commandFunction;
        }

        public async Task HandleAsync(HttpContext context)
        {
            var commandName = context.Request.Path.Value.Substring("/api/command/".Length);

            await _commandFunction.HandleAsync(commandName, context, _logger, context.RequestAborted);
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

#### Queries

```cs
using System.Threading.Tasks;
using CommandQuery.GoogleCloudFunctions;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    [FunctionsStartup(typeof(Startup))]
    public class Query : IHttpFunction
    {
        private readonly ILogger _logger;
        private readonly IQueryFunction _queryFunction;

        public Query(ILogger<Query> logger, IQueryFunction queryFunction)
        {
            _logger = logger;
            _queryFunction = queryFunction;
        }

        public async Task HandleAsync(HttpContext context)
        {
            var queryName = context.Request.Path.Value.Substring("/api/query/".Length);

            await _queryFunction.HandleAsync(queryName, context, _logger, context.RequestAborted);
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

#### Configuration

Configuration in `Startup.cs`:

```cs
using System.Text.Json;
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
            app.ApplicationServices.GetService<ICommandProcessor>().AssertConfigurationIsValid();
            app.ApplicationServices.GetService<IQueryProcessor>().AssertConfigurationIsValid();
        }
    }
}
```

The extension methods `AddCommandFunction` and `AddQueryFunction` will add functions and all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(Startup).Assembly` as a single argument.

#### Testing

```cs
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.GoogleCloudFunctions;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
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
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(null, serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(null, serviceProvider.GetService<IQueryFunction>());
            }

            [Test]
            public async Task should_work()
            {
                var context = GetHttpContext("BarQuery", "POST", content: "{ \"Id\": 1 }");

                await Subject.HandleAsync(context);
                var value = await context.Response.AsAsync<Bar>();

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var context = GetHttpContext("FailQuery", "POST", content: "{ \"Id\": 1 }");

                await Subject.HandleAsync(context);

                await context.Response.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            Query Subject;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(null, serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(null, serviceProvider.GetService<IQueryFunction>());
            }

            [Test]
            public async Task should_work()
            {
                var context = GetHttpContext("BarQuery", "GET", query: new Dictionary<string, string> { { "Id", "1" } });

                await Subject.HandleAsync(context);
                var value = await context.Response.AsAsync<Bar>();

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var context = GetHttpContext("FailQuery", "GET", query: new Dictionary<string, string> { { "Id", "1" } });

                await Subject.HandleAsync(context);

                await context.Response.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            Query Subject;
        }

        static HttpContext GetHttpContext(string queryName, string method, string content = null, Dictionary<string, string> query = null)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = new PathString("/api/query/" + queryName);
            context.Request.Method = method;

            if (content != null)
            {
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(content));
            }

            if (query != null)
            {
                context.Request.QueryString = new QueryString(QueryHelpers.AddQueryString("", query));
            }

            context.Response.Body = new MemoryStream();

            return context;
        }
    }
}
```

#### Samples

* [CommandQuery.Sample.GoogleCloudFunctions](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.GoogleCloudFunctions)
* [CommandQuery.Sample.GoogleCloudFunctions.Tests](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.GoogleCloudFunctions.Tests)
