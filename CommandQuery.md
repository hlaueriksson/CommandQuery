# CommandQuery

> Command Query Separation for .NET Framework and .NET Standard ⚙️

## Installation

| NuGet            |       | [![CommandQuery][1]][2]                                       |
| :--------------- | ----: | :------------------------------------------------------------ |
| Package Manager  | `PM>` | `Install-Package CommandQuery -Version 1.0.0`                 |
| .NET CLI         | `>`   | `dotnet add package CommandQuery --version 1.0.0`             |
| PackageReference |       | `<PackageReference Include="CommandQuery" Version="1.0.0" />` |
| Paket CLI        | `>`   | `paket add CommandQuery --version 1.0.0`                      |

[1]: https://img.shields.io/nuget/v/CommandQuery.svg?label=CommandQuery
[2]: https://www.nuget.org/packages/CommandQuery

## Sample Code

[`CommandQuery.Sample.Contracts`](/samples/CommandQuery.Sample.Contracts)

[`CommandQuery.Sample.Handlers`](/samples/CommandQuery.Sample.Handlers)

## Commands

> Commands: Change the state of a system but do not return a value.
>
> — <cite>[Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)</cite>

Create a `Command` and `CommandHandler`:

```csharp
public class FooCommand : ICommand
{
    public string Value { get; set; }
}

public class FooCommandHandler : ICommandHandler<FooCommand>
{
    private readonly ICultureService _cultureService;

    public FooCommandHandler(ICultureService cultureService)
    {
        _cultureService = cultureService;
    }

    public async Task HandleAsync(FooCommand command)
    {
        if (command.Value == null) throw new FooCommandException("Value cannot be null", 1337, "Try setting the value to 'en-US'");

        _cultureService.SetCurrentCulture(command.Value);

        await Task.CompletedTask;
    }
}
```

Commands implements the marker interface `ICommand` and command handlers implements `ICommandHandler<in TCommand>`.

This example uses the dependency `ICultureService` to set the current culture to the given command value.

The dogmatic approach to commands, that they *do not return a value*, can be inconvenient.
`CommandQuery` has a more pragmatic take and also supports commands with result.

Commands with result:

```csharp
public class BazCommand : ICommand<Baz>
{
    public string Value { get; set; }
}

public class Baz
{
    public bool Success { get; set; }
}

public class BazCommandHandler : ICommandHandler<BazCommand, Baz>
{
    private readonly ICultureService _cultureService;

    public BazCommandHandler(ICultureService cultureService)
    {
        _cultureService = cultureService;
    }

    public async Task<Baz> HandleAsync(BazCommand command)
    {
        var result = new Baz();

        try
        {
            _cultureService.SetCurrentCulture(command.Value);

            result.Success = true;
        }
        catch
        {
            // TODO: log
        }

        return await Task.FromResult(result);
    }
}
```

Commands with result implements the marker interface `ICommand<TResult>` and command handlers implements `ICommandHandler<in TCommand, TResult>`.

## Queries

> Queries: Return a result and do not change the observable state of the system (are free of side effects).
>
> — <cite>[Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)</cite>

Create a `Query`, `QueryHandler` and `Result`:

```csharp
public class Bar
{
    public int Id { get; set; }

    public string Value { get; set; }
}

public class BarQuery : IQuery<Bar>
{
    public int Id { get; set; }
}

public class BarQueryHandler : IQueryHandler<BarQuery, Bar>
{
    private readonly IDateTimeProxy _dateTime;

    public BarQueryHandler(IDateTimeProxy dateTime)
    {
        _dateTime = dateTime;
    }

    public async Task<Bar> HandleAsync(BarQuery query)
    {
        var result = new Bar { Id = query.Id, Value = _dateTime.Now.ToString("F") };

        return await Task.FromResult(result);
    }
}
```

Queries implements the marker interface `IQuery<TResult>` and query handlers implements `IQueryHandler<in TQuery, TResult>`.

This example uses the dependency `IDateTimeProxy` to return a result with the current time.
