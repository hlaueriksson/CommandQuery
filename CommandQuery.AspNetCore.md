### CommandQuery.AspNetCore 🌐

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml) [![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

> Command Query Separation for ASP.NET Core

* Provides generic actions for handling the execution of commands and queries
* Enables APIs based on HTTP `POST` and `GET`

#### Get Started

1. Create a new **ASP.NET Core** project
   * [Tutorial](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio)
2. Install the `CommandQuery.AspNetCore` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AspNetCore)
   * `PM>` `Install-Package CommandQuery.AspNetCore`
3. Create commands and command handlers
   * Implement `ICommand` and `ICommandHandler<in TCommand>`
   * Or `ICommand<TResult>` and `ICommandHandler<in TCommand, TResult>`
4. Create queries and query handlers
   * Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`
5. Configure services in `Startup.cs`

#### Configuration

Configuration in `Startup.cs`:

```cs
using CommandQuery.AspNetCore;
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
using Microsoft.OpenApi.Models;

namespace CommandQuery.Sample.AspNetCore.V5
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
            // Add commands and queries
            services.AddCommandControllers(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
            services.AddQueryControllers(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

            // Add handler dependencies
            services.AddTransient<IDateTimeProxy, DateTimeProxy>();
            services.AddTransient<ICultureService, CultureService>();

            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v5", new OpenApiInfo { Title = "CommandQuery.Sample.AspNetCore.V5", Version = "v5" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Swagger
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v5/swagger.json", "CommandQuery.Sample.AspNetCore.V5"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Validation
            app.ApplicationServices.GetService<ICommandProcessor>().AssertConfigurationIsValid();
            app.ApplicationServices.GetService<IQueryProcessor>().AssertConfigurationIsValid();
        }
    }
}
```

The extension methods `AddCommandControllers` and `AddQueryControllers` will add controllers and all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(Startup).Assembly` as a single argument.

#### Commands

The action method from the generated controller will handle commands:

```cs
/// <summary>
/// Handle a command.
/// </summary>
/// <param name="command">The command.</param>
/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
/// <returns>The result for status code <c>200</c>, or an error for status code <c>400</c> and <c>500</c>.</returns>
[HttpPost]
public async Task<IActionResult> HandleAsync(TCommand command, CancellationToken cancellationToken)
```

* The action is requested via HTTP `POST` with the Content-Type `application/json` in the header
* The name of the command is the slug of the URL
* The command itself is provided as JSON in the body
* If the command succeeds; the response is empty with the HTTP status code `200`
* If the command fails; the response is an error message with the HTTP status code `400` or `500`

Commands with result:

```cs
/// <summary>
/// Handle a command.
/// </summary>
/// <param name="command">The command.</param>
/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
/// <returns>The result for status code <c>200</c>, or an error for status code <c>400</c> and <c>500</c>.</returns>
[HttpPost]
public async Task<IActionResult> HandleAsync(TCommand command, CancellationToken cancellationToken)
```

* If the command succeeds; the response is the result as JSON with the HTTP status code `200`.
* If the command fails; the response is an error message with the HTTP status code `400` or `500`.

#### Queries

The action methods from the generated controller will handle queries:

```cs
/// <summary>
/// Handle a query.
/// </summary>
/// <param name="query">The query.</param>
/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
/// <returns>The result + 200, 400 or 500.</returns>
[HttpPost]
public async Task<IActionResult> HandlePostAsync(TQuery query, CancellationToken cancellationToken)
```

```cs
/// <summary>
/// Handle a query.
/// </summary>
/// <param name="query">The query.</param>
/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
/// <returns>The result + 200, 400 or 500.</returns>
[HttpGet]
public async Task<IActionResult> HandleGetAsync([FromQuery] TQuery query, CancellationToken cancellationToken)
```

* The action is requested via:
  * HTTP `POST` with the Content-Type `application/json` in the header and the query itself as JSON in the body
  * HTTP `GET` and the query itself as query string parameters in the URL
* The name of the query is the slug of the URL.
* If the query succeeds; the response is the result as JSON with the HTTP status code `200`.
* If the query fails; the response is an error message with the HTTP status code `400` or `500`.

#### Testing

You can [integration test](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0) your controllers and command/query handlers with the `Microsoft.AspNetCore.Mvc.Testing` package.

```cs
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.V5.Tests
{
    public class QueryControllerTests
    {
        public class when_using_the_real_controller_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                Factory = new WebApplicationFactory<Startup>();
                Client = Factory.CreateClient();
            }

            [TearDown]
            public void TearDown()
            {
                Client.Dispose();
                Factory.Dispose();
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

            [Test]
            public async Task should_handle_errors()
            {
                var content = new StringContent("{ \"Id\": 1 }", Encoding.UTF8, "application/json");

                var result = await Client.PostAsync("/api/query/FailQuery", content);

                result.StatusCode.Should().Be(HttpStatusCode.NotFound);
                (await result.Content.ReadAsStringAsync()).Should().BeEmpty();
            }

            WebApplicationFactory<Startup> Factory;
            HttpClient Client;
        }

        public class when_using_the_real_controller_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                Factory = new WebApplicationFactory<Startup>();
                Client = Factory.CreateClient();
            }

            [TearDown]
            public void TearDown()
            {
                Client.Dispose();
                Factory.Dispose();
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

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Client.GetAsync("/api/query/FailQuery?Id=1");

                result.StatusCode.Should().Be(HttpStatusCode.NotFound);
                (await result.Content.ReadAsStringAsync()).Should().BeEmpty();
            }

            WebApplicationFactory<Startup> Factory;
            HttpClient Client;
        }
    }
}
```

#### Samples

* [CommandQuery.Sample.AspNetCore.V3](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AspNetCore.V3)
* [CommandQuery.Sample.AspNetCore.V3.Tests](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AspNetCore.V3.Tests)
* [CommandQuery.Sample.AspNetCore.V5](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AspNetCore.V5)
* [CommandQuery.Sample.AspNetCore.V5.Tests](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AspNetCore.V5.Tests)
