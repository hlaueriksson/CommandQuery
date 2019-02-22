# CommandQuery.AzureFunctions

> Command Query Separation for Azure Functions ⚡

* Provides generic function support for commands and queries with *HTTPTriggers*
* Enables APIs based on HTTP `POST` and `GET`

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.AzureFunctions.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.AzureFunctions.svg)](https://www.nuget.org/packages/CommandQuery.AzureFunctions)

`PM>` `Install-Package CommandQuery.AzureFunctions`

`>` `dotnet add package CommandQuery.AzureFunctions`

## Sample Code

* *Visual Studio*:
    * [`CommandQuery.Sample.AzureFunctions.Vs1`](/samples/CommandQuery.Sample.AzureFunctions.Vs1) - Azure Functions v1 (.NET Framework)
    * [`CommandQuery.Sample.AzureFunctions.Vs1.Tests`](/samples/CommandQuery.Sample.AzureFunctions.Vs1.Tests)
    * [`CommandQuery.Sample.AzureFunctions.Vs2`](/samples/CommandQuery.Sample.AzureFunctions.Vs2) - Azure Functions v2 (.NET Core)
    * [`CommandQuery.Sample.AzureFunctions.Vs2.Tests`](/samples/CommandQuery.Sample.AzureFunctions.Vs2.Tests)
* *Visual Studio Code*:
    * [`CommandQuery.Sample.AzureFunctions.VsCode1`](/samples/CommandQuery.Sample.AzureFunctions.VsCode1) - Azure Functions v1 (.NET Framework)
    * [`CommandQuery.Sample.AzureFunctions.VsCode2`](/samples/CommandQuery.Sample.AzureFunctions.VsCode2) - Azure Functions v2 (.NET Core)

## Get Started in *Visual Studio*

0. Install **Azure Functions and Web Jobs Tools**
    * [https://marketplace.visualstudio.com/items?itemName=VisualStudioWebandAzureTools.AzureFunctionsandWebJobsTools](https://marketplace.visualstudio.com/items?itemName=VisualStudioWebandAzureTools.AzureFunctionsandWebJobsTools)
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

## Get Started in *Visual Studio Code*

0. Install **Azure Functions**
    * [https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)
1. Create a new **Azure Functions** project
	* [Tutorial](https://code.visualstudio.com/tutorials/functions-extension/getting-started)
2. Install the `CommandQuery.AzureFunctions` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AzureFunctions/)
	* `dotnet add package CommandQuery.AzureFunctions`
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

## Commands

Add a `Command` function in *Azure Functions v1 (.NET Framework)*:

```csharp
using System.Net.Http;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AzureFunctions.Vs1
{
    public static class Command
    {
        private static readonly CommandFunction Func = new CommandFunction(
            new[] { typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly }
                .GetCommandProcessor(GetServiceCollection()));

        [FunctionName("Command")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequestMessage req, TraceWriter log, string commandName)
        {
            return await Func.Handle(commandName, req, log);
        }

        private static IServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection();
            // Add handler dependencies
            services.AddTransient<ICultureService, CultureService>();

            return services;
        }
    }
}
```

Add a `Command` function in *Azure Functions v2 (.NET Core)*:

```csharp
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Sample.AzureFunctions.Vs2
{
    public static class Command
    {
        private static readonly CommandFunction Func = new CommandFunction(
            new[] { typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly }
                .GetCommandProcessor(GetServiceCollection()));

        [FunctionName("Command")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequest req, ILogger log, string commandName)
        {
            return await Func.Handle(commandName, req, log);
        }

        private static IServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection();
            // Add handler dependencies
            services.AddTransient<ICultureService, CultureService>();

            return services;
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

`curl -X POST -d "{'Value':'sv-SE'}" http://localhost:7071/api/command/FooCommand --header "Content-Type:application/json"`

## Queries

Add a `Query` function in *Azure Functions v1 (.NET Framework)*:

```csharp
using System.Net.Http;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AzureFunctions.Vs1
{
    public static class Query
    {
        private static readonly QueryFunction Func = new QueryFunction(
            new[] { typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly }
                .GetQueryProcessor(GetServiceCollection()));

        [FunctionName("Query")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query/{queryName}")] HttpRequestMessage req, TraceWriter log, string queryName)
        {
            return await Func.Handle(queryName, req, log);
        }

        private static IServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection();
            // Add handler dependencies
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();

            return services;
        }
    }
}
```

Add a `Query` function in *Azure Functions v2 (.NET Core)*:

```csharp
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Sample.AzureFunctions.Vs2
{
    public static class Query
    {
        private static readonly QueryFunction Func = new QueryFunction(
            new[] { typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly }
                .GetQueryProcessor(GetServiceCollection()));

        [FunctionName("Query")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query/{queryName}")] HttpRequest req, ILogger log, string queryName)
        {
            return await Func.Handle(queryName, req, log);
        }

        private static IServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection();
            // Add handler dependencies
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();

            return services;
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

Example of query requests via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Id':1}" http://localhost:7071/api/query/BarQuery --header "Content-Type:application/json"`

`curl -X GET http://localhost:7071/api/query/BarQuery?Id=1`

## Testing

Test commands in *Azure Functions v1 (.NET Framework)*:

```csharp
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.Vs1.Tests
{
    public class CommandTests
    {
        [Test]
        public async Task should_work()
        {
            var req = GetHttpRequest("{ 'Value': 'Foo' }");
            var log = new FakeTraceWriter();

            var result = await Command.Run(req, log, "FooCommand");

            result.Should().NotBeNull();
        }

        static HttpRequestMessage GetHttpRequest(string content)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage();
            request.SetConfiguration(config);
            request.Content = new StringContent(content);

            return request;
        }
    }
}
```

Test queries in *Azure Functions v2 (.NET Core)*:

```csharp
using System.IO;
using System.Threading.Tasks;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.Vs2.Tests
{
    public class QueryTests
    {
        [SetUp]
        public void SetUp()
        {
            Req = GetHttpRequest("POST", content: "{ 'Id': 1 }");
            Log = new Mock<ILogger>().Object;
        }

        [Test]
        public async Task should_work()
        {
            var result = await Query.Run(Req, Log, "BarQuery") as OkObjectResult;
            var value = result.Value as Bar;

            value.Id.Should().Be(1);
            value.Value.Should().NotBeEmpty();
        }

        DefaultHttpRequest GetHttpRequest(string method, string content = null)
        {
            var httpContext = new DefaultHttpContext();

            if (content != null)
            {
                httpContext.Features.Get<IHttpRequestFeature>().Body =
                    new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            }

            var request = new DefaultHttpRequest(httpContext) { Method = method };

            return request;
        }

        DefaultHttpRequest Req;
        ILogger Log;
    }
}
```
