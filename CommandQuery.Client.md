# CommandQuery.Client

> Clients for CommandQuery

## Installation

| NuGet            |       | [![CommandQuery.Client][1]][2]                                       |
| :--------------- | ----: | :------------------------------------------------------------------- |
| Package Manager  | `PM>` | `Install-Package CommandQuery.Client -Version 0.9.0`                 |
| .NET CLI         | `>`   | `dotnet add package CommandQuery.Client --version 0.9.0`             |
| PackageReference |       | `<PackageReference Include="CommandQuery.Client" Version="0.9.0" />` |
| Paket CLI        | `>`   | `paket add CommandQuery.Client --version 0.9.0`                      |

[1]: https://img.shields.io/nuget/v/CommandQuery.Client.svg?label=CommandQuery.Client
[2]: https://www.nuget.org/packages/CommandQuery.Client

## Sample Code

[`CommandQuery.Sample.Client`](/samples/CommandQuery.Sample.Client)

## Commands

Create a `CommandClient` and invoke `Post`:

```csharp
var commandClient = new CommandClient("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/command/");

commandClient.Post(new FooCommand { Value = "sv-SE" });
await commandClient.PostAsync(new FooCommand { Value = "en-GB" });
```

Commands with result:

```csharp
var result = commandClient.Post(new BazCommand { Value = "sv-SE" });
result = await commandClient.PostAsync(new BazCommand { Value = "en-GB" });
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
