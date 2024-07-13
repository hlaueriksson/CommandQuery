# CommandQuery.AspNetCore 🌐

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml) [![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

> Command Query Separation for ASP.NET Core

* Provides generic actions for handling the execution of commands and queries
* Enables APIs based on HTTP `POST` and `GET`

## Get Started

1. Create a new **ASP.NET Core** project
   * [Tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio)
2. Install the `CommandQuery.AspNetCore` package from [NuGet](https://www.nuget.org/packages/CommandQuery.AspNetCore)
   * `PM>` `Install-Package CommandQuery.AspNetCore`
3. Create commands and command handlers
   * Implement `ICommand` and `ICommandHandler<in TCommand>`
   * Or `ICommand<TResult>` and `ICommandHandler<in TCommand, TResult>`
4. Create queries and query handlers
   * Implement `IQuery<TResult>` and `IQueryHandler<in TQuery, TResult>`
5. Configure services in `Startup.cs`

![Add a new project - ASP.NET Core Web API](https://raw.githubusercontent.com/hlaueriksson/CommandQuery/master/vs-new-project-aspnet-core-web-api.png)

Choose:

* .NET 8.0 (Long Term Support)
* Use controllers

## Configuration

Configuration in `Program.cs`:

```cs
using CommandQuery;
using CommandQuery.AspNetCore;
using CommandQuery.Sample.Contracts.Commands;
using CommandQuery.Sample.Contracts.Queries;
using CommandQuery.Sample.Handlers;
using CommandQuery.Sample.Handlers.Commands;
using CommandQuery.Sample.Handlers.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add commands and queries
builder.Services.AddCommandControllers(typeof(FooCommandHandler).Assembly, typeof(FooCommand).Assembly);
builder.Services.AddQueryControllers(typeof(BarQueryHandler).Assembly, typeof(BarQuery).Assembly);

// Add handler dependencies
builder.Services.AddTransient<IDateTimeProxy, DateTimeProxy>();
builder.Services.AddTransient<ICultureService, CultureService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Validation
app.Services.GetService<ICommandProcessor>()!.AssertConfigurationIsValid();
app.Services.GetService<IQueryProcessor>()!.AssertConfigurationIsValid();

app.Run();
```

The extension methods `AddCommandControllers` and `AddQueryControllers` will add controllers and all command/query handlers in the given assemblies to the IoC container.
You can pass in a `params` array of `Assembly` arguments if your handlers are located in different projects.
If you only have one project you can use `typeof(Startup).Assembly` as a single argument.

## Commands

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

## Queries

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

## Testing

You can [integration test](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0) your controllers and command/query handlers with the `Microsoft.AspNetCore.Mvc.Testing` package.

```cs
using System.Net;
using System.Net.Http.Json;
using CommandQuery.Sample.Contracts.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.Tests
{
    public class CommandControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            Factory = new WebApplicationFactory<Program>();
            Client = Factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            Client.Dispose();
            Factory.Dispose();
        }

        [Test]
        public async Task should_handle_command()
        {
            var response = await Client.PostAsJsonAsync("/api/command/FooCommand", new FooCommand { Value = "Foo" });
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task should_handle_errors()
        {
            var response = await Client.PostAsJsonAsync("/api/command/FooCommand", new FooCommand { Value = "" });
            await response.ShouldBeErrorAsync("Value cannot be null or empty");
        }

        WebApplicationFactory<Program> Factory = null!;
        HttpClient Client = null!;
    }
}
```

## Samples

* [CommandQuery.Sample.AspNetCore](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AspNetCore)
* [CommandQuery.Sample.AspNetCore.Tests](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.AspNetCore.Tests)
