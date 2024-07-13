# CommandQuery.Abstractions ⚙️

[![build](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml/badge.svg)](https://github.com/hlaueriksson/CommandQuery/actions/workflows/build.yml) [![CodeFactor](https://codefactor.io/repository/github/hlaueriksson/commandquery/badge)](https://codefactor.io/repository/github/hlaueriksson/commandquery)

> Command Query Separation abstractions for .NET and C#

## Commands

```cs
public interface ICommand;

public interface ICommand<TResult>;
```

```cs
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
```

## Queries

```cs
public interface IQuery<TResult>;
```

```cs
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
```

## Error

```cs
public interface IError
{
    public string? Message { get; }

    public Dictionary<string, object>? Details { get; }
}
```

## Samples

* [CommandQuery.Sample.Contracts](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.Contracts)
* [CommandQuery.Sample.Handlers](https://github.com/hlaueriksson/CommandQuery/tree/master/samples/CommandQuery.Sample.Handlers)
