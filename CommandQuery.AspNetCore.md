# CommandQuery.AspNetCore

> Command Query Separation for ASP.NET Core 🌐

* Provides generic actions for handling the execution of commands and queries
* Enables APIs based on HTTP `POST` and `GET`

## Installation

| NuGet            |       | [![CommandQuery.AspNetCore][1]][2]                                       |
| :--------------- | ----: | :----------------------------------------------------------------------- |
| Package Manager  | `PM>` | `Install-Package CommandQuery.AspNetCore -Version 0.9.0`                 |
| .NET CLI         | `>`   | `dotnet add package CommandQuery.AspNetCore --version 0.9.0`             |
| PackageReference |       | `<PackageReference Include="CommandQuery.AspNetCore" Version="0.9.0" />` |
| Paket CLI        | `>`   | `paket add CommandQuery.AspNetCore --version 0.9.0`                      |

[1]: https://img.shields.io/nuget/v/CommandQuery.AspNetCore.svg?label=CommandQuery.AspNetCore
[2]: https://www.nuget.org/packages/CommandQuery.AspNetCore

## Sample Code

[`CommandQuery.Sample.AspNetCore`](/samples/CommandQuery.Sample.AspNetCore)

[`CommandQuery.Sample.AspNetCore.Tests`](/samples/CommandQuery.Sample.AspNetCore.Tests)

## Get Started

1. Create a new **ASP.NET Core 2.0** project
	* [Tutorials](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-2.1)
2. Install the `CommandQuery.AspNetCore` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AspNetCore)
	* `PM>` `Install-Package CommandQuery.AspNetCore`
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
public async Task<IActionResult> Handle(string commandName, [FromBody] Newtonsoft.Json.Linq.JObject json)
```

* The action is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the command is the slug of the URL.
* The command itself is provided as JSON in the body.
* If the command succeeds; the response is empty with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Example of a command request via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Value':'sv-SE'}" http://localhost:57857/api/command/FooCommand --header "Content-Type:application/json"`

## Queries

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

The action methods from the base class will handle all queries:

```csharp
[HttpPost]
[Route("{queryName}")]
public async Task<IActionResult> HandlePost(string queryName, [FromBody] Newtonsoft.Json.Linq.JObject json)
```

```csharp
[HttpGet]
[Route("{queryName}")]
public async Task<IActionResult> HandleGet(string queryName)
```

* The action is requested via:
    * HTTP `POST` with the Content-Type `application/json` in the header and the query itself as JSON in the body
    * HTTP `GET` and the query itself as query string parameters in the URL
* The name of the query is the slug of the URL.
* If the query succeeds; the response is the result as JSON with the HTTP status code `200`.
* If the query fails; the response is an error message with the HTTP status code `400` or `500`.

Example of query requests via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Id':1}" http://localhost:57857/api/query/BarQuery --header "Content-Type:application/json"`

`curl -X GET http://localhost:57857/api/query/BarQuery?Id=1`

## Configuration

Configuration in `Startup.cs`:

```csharp
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Add commands and queries.
            services.AddCommands(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
            services.AddQueries(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

            // Add handler dependencies
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();
            services.AddTransient<ICultureService, CultureService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
```

Register command/query handlers and other dependencies in the `ConfigureServices` method.

The extension methods `AddCommands` and `AddQueries` will add all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(Startup).Assembly` as a single argument.

## Testing

You can [integration test](https://docs.microsoft.com/en-us/aspnet/core/testing/integration-testing) your controllers and command/query handlers with the `Microsoft.AspNetCore.TestHost`.

Test commands:

```csharp
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.Tests
{
    public class CommandControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = Server.CreateClient();
        }

        [Test]
        public async Task should_work()
        {
            var content = new StringContent("{ 'Value': 'Foo' }", Encoding.UTF8, "application/json");
            var result = await Client.PostAsync("/api/command/FooCommand", content);

            result.EnsureSuccessStatusCode();
            (await result.Content.ReadAsStringAsync()).Should().BeEmpty();
        }

        TestServer Server;
        HttpClient Client;
    }
}
```

Test queries:

```csharp
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.Tests
{
    public class QueryControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = Server.CreateClient();
        }

        [Test]
        public async Task should_work()
        {
            var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
            var result = await Client.PostAsync("/api/query/BarQuery", content);
            var value = await result.Content.ReadAsAsync<Bar>();

            result.EnsureSuccessStatusCode();
            value.Id.Should().Be(1);
            value.Value.Should().NotBeEmpty();
        }

        TestServer Server;
        HttpClient Client;
    }
}
```
