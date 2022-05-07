# CommandQuery ⚙️

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml) [![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

> Command Query Separation for .NET and C#

## Commands

> Commands change the state of a system but _[traditionally]_ do not return a value. They write (create, update, delete) data.

Commands implement the marker interface `ICommand` and command handlers implement `ICommandHandler<in TCommand>`.

```cs
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

    public async Task HandleAsync(FooCommand command, CancellationToken cancellationToken)
    {
        if (command.Value == null) throw new FooCommandException("Value cannot be null", 1337, "Try setting the value to 'en-US'");

        _cultureService.SetCurrentCulture(command.Value);

        await Task.CompletedTask;
    }
}
```

Commands can also return a result.

```cs
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

    public async Task<Baz> HandleAsync(BazCommand command, CancellationToken cancellationToken)
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

Commands with result implement the marker interface `ICommand<TResult>` and command handlers implement `ICommandHandler<in TCommand, TResult>`.

## Queries

> Queries return a result and do not change the observable state of the system (are free of side effects). They read and return data.

Queries implement the marker interface `IQuery<TResult>` and query handlers implement `IQueryHandler<in TQuery, TResult>`.

```cs
public class BarQuery : IQuery<Bar>
{
    public int Id { get; set; }
}

public class Bar
{
    public int Id { get; set; }

    public string Value { get; set; }
}

public class BarQueryHandler : IQueryHandler<BarQuery, Bar>
{
    private readonly IDateTimeProxy _dateTime;

    public BarQueryHandler(IDateTimeProxy dateTime)
    {
        _dateTime = dateTime;
    }

    public async Task<Bar> HandleAsync(BarQuery query, CancellationToken cancellationToken)
    {
        var result = new Bar { Id = query.Id, Value = _dateTime.Now.ToString("F") };

        return await Task.FromResult(result);
    }
}
```

## Samples

* [CommandQuery.Sample.Contracts](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.Contracts)
* [CommandQuery.Sample.Handlers](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.Handlers)
