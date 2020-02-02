# CommandQuery.AspNetCore

> Command Query Separation for ASP.NET Core 🌐

* Provides generic actions for handling the execution of commands and queries
* Enables APIs based on HTTP `POST` and `GET`

## Installation

| NuGet            |       | [![CommandQuery.AspNetCore][1]][2]                                       |
| :--------------- | ----: | :----------------------------------------------------------------------- |
| Package Manager  | `PM>` | `Install-Package CommandQuery.AspNetCore -Version 1.0.0`                 |
| .NET CLI         | `>`   | `dotnet add package CommandQuery.AspNetCore --version 1.0.0`             |
| PackageReference |       | `<PackageReference Include="CommandQuery.AspNetCore" Version="1.0.0" />` |
| Paket CLI        | `>`   | `paket add CommandQuery.AspNetCore --version 1.0.0`                      |

[1]: https://img.shields.io/nuget/v/CommandQuery.AspNetCore.svg?label=CommandQuery.AspNetCore
[2]: https://www.nuget.org/packages/CommandQuery.AspNetCore

## Sample Code

[`CommandQuery.Sample.AspNetCore.V3`](/samples/CommandQuery.Sample.AspNetCore.V3)

[`CommandQuery.Sample.AspNetCore.V3.Tests`](/samples/CommandQuery.Sample.AspNetCore.V3.Tests)

## Get Started

1. Create a new **ASP.NET Core 3.1** project
	* [Tutorials](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-3.1)
2. Install the `CommandQuery.AspNetCore` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AspNetCore)
	* `PM>` `Install-Package CommandQuery.AspNetCore`
3. Create commands and command handlers
	* Implement `ICommand` and `ICommandHandler<in TCommand>`
	* Or `ICommand<TResult>` and `ICommandHandler<in TCommand, TResult>`
4. Create queries and query handlers
	* Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`
5. Configure services in `Startup.cs`

## Configuration

Configuration in `Startup.cs`:

```csharp
using CommandQuery.AspNetCore;
using CommandQuery.DependencyInjection;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommandQuery.Sample.AspNetCore.V3
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
            services
                .AddControllers(options => options.Conventions.Add(new CommandQueryControllerModelConvention()))
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new CommandControllerFeatureProvider(typeof(FooCommand).Assembly));
                    manager.FeatureProviders.Add(new QueryControllerFeatureProvider(typeof(BarQuery).Assembly));
                });

            // Add commands and queries
            services.AddCommands(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
            services.AddQueries(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

            // Add handler dependencies
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();
            services.AddTransient<ICultureService, CultureService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```

Add controllers for commands and queries via the classes `CommandQueryControllerModelConvention`, `CommandControllerFeatureProvider` and `QueryControllerFeatureProvider` in the `ConfigureServices` method.
You can pass in a `params` array of `Assembly` arguments to the feature providers classes, if your commands/queries are located in different projects.

:sparkles: Each command and query in the provided assemblies will automatically get a generated controller.
There is no need to create a controller for commands and queries yourself anymore.

Continue by registering command/query handlers and other dependencies.

The extension methods `AddCommands` and `AddQueries` will add all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(Startup).Assembly` as a single argument.

## Commands

The action method from the generated controller will handle the command in question:

```csharp
/// <summary>
/// Handle a command.
/// </summary>
/// <param name="command">The command</param>
/// <returns>200, 400 or 500</returns>
[HttpPost]
public async Task<IActionResult> Handle(TCommand command)
```

* The action is requested via HTTP `POST` with the Content-Type `application/json` in the header.
* The name of the command is the slug of the URL.
* The command itself is provided as JSON in the body.
* If the command succeeds; the response is empty with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Commands with result:

```csharp
/// <summary>
/// Handle a command.
/// </summary>
/// <param name="command">The command</param>
/// <returns>The result + 200, 400 or 500</returns>
[HttpPost]
public async Task<IActionResult> Handle(TCommand command)
```

* If the command succeeds; the response is the result as JSON with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

Example of a command request via [curl](https://curl.haxx.se):

`curl -X POST -d "{'Value':'sv-SE'}" http://localhost:57857/api/command/FooCommand --header "Content-Type:application/json"`

## Queries

The action methods from the generated controller will handle the query in question:

```csharp
/// <summary>
/// Handle a query.
/// </summary>
/// <param name="query">The query</param>
/// <returns>The result + 200, 400 or 500</returns>
[HttpPost]
public async Task<IActionResult> HandlePost(TQuery query)
```

```csharp
/// <summary>
/// Handle a query.
/// </summary>
/// <param name="query">The query</param>
/// <returns>The result + 200, 400 or 500</returns>
[HttpGet]
public async Task<IActionResult> HandleGet([FromQuery] TQuery query)
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

## Testing

You can [integration test](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1) your controllers and command/query handlers with the `Microsoft.AspNetCore.Mvc.Testing` package.

Test commands:

```csharp
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.V3.Tests
{
    public class CommandControllerTests
    {
        public class when_using_the_real_controller
        {
            [SetUp]
            public void SetUp()
            {
                var factory = new WebApplicationFactory<Startup>();
                Client = factory.CreateClient();
            }

            [Test]
            public async Task should_work()
            {
                var content = new StringContent("{ \"Value\": \"Foo\" }", Encoding.UTF8, "application/json");
                var result = await Client.PostAsync("/api/command/FooCommand", content);

                result.EnsureSuccessStatusCode();
                (await result.Content.ReadAsStringAsync()).Should().BeEmpty();
            }

            HttpClient Client;
        }
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
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.V3.Tests
{
    public class QueryControllerTests
    {
        public class when_using_the_real_controller_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                var factory = new WebApplicationFactory<Startup>();
                Client = factory.CreateClient();
            }

            [Test]
            public async Task should_work()
            {
                var content = new StringContent("{ \"Id\": 1 }", Encoding.UTF8, "application/json");
                var result = await Client.PostAsync("/api/query/BarQuery", content);
                var value = await result.Content.ReadAsAsync<Bar>();

                result.EnsureSuccessStatusCode();
                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            HttpClient Client;
        }

        public class when_using_the_real_controller_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                var factory = new WebApplicationFactory<Startup>();
                Client = factory.CreateClient();
            }

            [Test]
            public async Task should_work()
            {
                var result = await Client.GetAsync("/api/query/BarQuery?Id=1");
                var value = await result.Content.ReadAsAsync<Bar>();

                result.EnsureSuccessStatusCode();
                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            HttpClient Client;
        }
    }
}
```
