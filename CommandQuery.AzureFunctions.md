### CommandQuery.AzureFunctions ⚡

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml) [![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

> Command Query Separation for Azure Functions

* Provides generic function support for commands and queries with *HTTPTriggers*
* Enables APIs based on HTTP `POST` and `GET`

#### Get Started

0. Install **Azure Functions and Web Jobs Tools**
   * [https://marketplace.visualstudio.com/items?itemName=VisualStudioWebandAzureTools.AzureFunctionsandWebJobsTools](https://marketplace.visualstudio.com/items?itemName=VisualStudioWebandAzureTools.AzureFunctionsandWebJobsTools)
1. Create a new **Azure Functions** project
   * [Tutorial](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio)
2. Install the `CommandQuery.AzureFunctions` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AzureFunctions/)
   * `PM>` `Install-Package CommandQuery.AzureFunctions`
3. Create functions
   * Preferably named `Command` and `Query`
4. Create commands and command handlers
   * Implement `ICommand` and `ICommandHandler<in TCommand>`
   * Or `ICommand<TResult>` and `ICommandHandler<in TCommand, TResult>`
5. Create queries and query handlers
   * Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`
6. Configure services in `Program.cs`

![Add a new project - Azure Functions](vs-new-project-azure-functions-1.png)

Choose:

* .NET 5 (Isolated)
* Http trigger

![Create a new Azure Functions Application](vs-new-project-azure-functions-2.png)

#### Commands

```cs
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.Sample.AzureFunctions.V5
{
    public class Command
    {
        private readonly ICommandFunction _commandFunction;

        public Command(ICommandFunction commandFunction)
        {
            _commandFunction = commandFunction;
        }

        [Function("Command")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequestData req, FunctionContext executionContext, string commandName)
        {
            var logger = executionContext.GetLogger("Command");

            return await _commandFunction.HandleAsync(commandName, req, logger);
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
using CommandQuery.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.Sample.AzureFunctions.V5
{
    public class Query
    {
        private readonly IQueryFunction _queryFunction;

        public Query(IQueryFunction queryFunction)
        {
            _queryFunction = queryFunction;
        }

        [Function("Query")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query/{queryName}")] HttpRequestData req, FunctionContext executionContext, string queryName)
        {
            var logger = executionContext.GetLogger("Query");

            return await _queryFunction.HandleAsync(queryName, req, logger);
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

Configuration in `Program.cs`:

```cs
using System.Text.Json;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommandQuery.Sample.AzureFunctions.V5
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(ConfigureServices)
                .Build();

            // Validation
            host.Services.GetService<ICommandProcessor>().AssertConfigurationIsValid();
            host.Services.GetService<IQueryProcessor>().AssertConfigurationIsValid();

            host.Run();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services
                //.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.Web));

                // Add commands and queries
                .AddCommandFunction(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly)
                .AddQueryFunction(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly)

                // Add handler dependencies
                .AddTransient<IDateTimeProxy, DateTimeProxy>()
                .AddTransient<ICultureService, CultureService>();
        }
    }
}
```

The extension methods `AddCommandFunction` and `AddQueryFunction` will add functions and all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(Program).Assembly` as a single argument.

#### Testing

```cs
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.V5.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
                Program.ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var context = new Mock<FunctionContext>();
                context.SetupProperty(c => c.InstanceServices, serviceProvider);
                ExecutionContext = context.Object;

                Subject = new Query(serviceProvider.GetService<IQueryFunction>());
            }

            [Test]
            public async Task should_work()
            {
                var req = GetHttpRequestData(ExecutionContext, "POST", content: "{ \"Id\": 1 }");

                var result = await Subject.Run(req, ExecutionContext, "BarQuery");
                var value = await result.AsAsync<Bar>();

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var req = GetHttpRequestData(ExecutionContext, "POST", content: "{ \"Id\": 1 }");

                var result = await Subject.Run(req, ExecutionContext, "FailQuery");

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            FunctionContext ExecutionContext;
            Query Subject;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
                Program.ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var context = new Mock<FunctionContext>();
                context.SetupProperty(c => c.InstanceServices, serviceProvider);
                ExecutionContext = context.Object;

                Subject = new Query(serviceProvider.GetService<IQueryFunction>());
            }

            [Test]
            public async Task should_work()
            {
                var req = GetHttpRequestData(ExecutionContext, "GET", url: "http://localhost?Id=1");

                var result = await Subject.Run(req, ExecutionContext, "BarQuery");
                var value = await result.AsAsync<Bar>();

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var req = GetHttpRequestData(ExecutionContext, "GET", url: "http://localhost?Id=1");

                var result = await Subject.Run(req, ExecutionContext, "FailQuery");

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            FunctionContext ExecutionContext;
            Query Subject;
        }

        static HttpRequestData GetHttpRequestData(FunctionContext executionContext, string method, string content = null, string url = null)
        {
            var request = new Mock<HttpRequestData>(executionContext);
            request.Setup(r => r.Method).Returns(method);

            if (content != null)
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                request.Setup(r => r.Body).Returns(stream);
            }

            if (url != null)
            {
                request.Setup(r => r.Url).Returns(new Uri(url));
            }

            request.Setup(r => r.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(executionContext);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode);
                response.SetupProperty(r => r.Body, new MemoryStream());
                return response.Object;
            });

            return request.Object;
        }
    }
}
```

#### Samples

* [CommandQuery.Sample.AzureFunctions.V3](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AzureFunctions.V3)
* [CommandQuery.Sample.AzureFunctions.V3.Tests](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AzureFunctions.V3.Tests)
* [CommandQuery.Sample.AzureFunctions.V5](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AzureFunctions.V5)
* [CommandQuery.Sample.AzureFunctions.V5.Tests](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AzureFunctions.V5.Tests)
