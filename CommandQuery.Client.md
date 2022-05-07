# CommandQuery.Client 🧰

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml) [![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

> Clients for CommandQuery APIs

## Commands

Create a `CommandClient` and invoke `PostAsync`:

```cs
var commandClient = new CommandClient("https://commandquery-sample-aspnetcore-v5.azurewebsites.net/api/command/");

await commandClient.PostAsync(new FooCommand { Value = "en-GB" });
```

Commands with result:

```cs
var result = await commandClient.PostAsync(new BazCommand { Value = "en-GB" });
```

## Queries

Create a `QueryClient` and invoke `PostAsync` or `GetAsync`:

```cs
var queryClient = new QueryClient("https://commandquery-sample-aspnetcore-v5.azurewebsites.net/api/query/");

var result = await queryClient.PostAsync(new BarQuery { Id = 1 });
result = await queryClient.GetAsync(new BarQuery { Id = 1 });
```

## Samples

* [CommandQuery.Sample.Client](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.Client)
