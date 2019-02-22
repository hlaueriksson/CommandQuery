# CommandQuery.Client

> Clients for CommandQuery

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.Client.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.Client.svg)](https://www.nuget.org/packages/CommandQuery.Client)

`PM>` `Install-Package CommandQuery.Client`

`>` `dotnet add package CommandQuery.Client`

## Sample Code

[`CommandQuery.Sample.Client`](/samples/CommandQuery.Sample.Client)

## Commands

Create a `CommandClient` and invoke `Post`:

```csharp
var commandClient = new CommandClient("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/command/");

commandClient.Post(new FooCommand { Value = "sv-SE" });
await commandClient.PostAsync(new FooCommand { Value = "en-GB" });
```

## Queries

Create a `QueryClient` and invoke `Post` or `Get`:

```csharp
var queryClient = new QueryClient("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/query/");

var result = queryClient.Post(new BarQuery { Id = 1 });
result = await queryClient.PostAsync(new BarQuery { Id = 1 });

result = queryClient.Get(new BarQuery { Id = 1 });
result = await queryClient.GetAsync(new BarQuery { Id = 1 });
```
