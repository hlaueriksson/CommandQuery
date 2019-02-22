# CommandQuery

> Command Query Separation for .NET Framework and .NET Standard ⚙️

[![NuGet](https://img.shields.io/nuget/v/CommandQuery.svg) ![NuGet](https://img.shields.io/nuget/dt/CommandQuery.svg)](https://www.nuget.org/packages/CommandQuery)

`PM>` `Install-Package CommandQuery`

`>` `dotnet add package CommandQuery`

## Sample Code

[`CommandQuery.Sample.Contracts`](/samples/CommandQuery.Sample.Contracts)

[`CommandQuery.Sample.Handlers`](/samples/CommandQuery.Sample.Handlers)

## Commands

> Commands: Change the state of a system but do not return a value.
>
> — <cite>[Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)</cite>

Create a `Command` and `CommandHandler`:

```csharp
using System.Threading.Tasks;

namespace CommandQuery.Sample.Commands
{
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
            _cultureService.SetCurrentCulture(command.Value);

            await Task.Delay(10); // TODO: do some real command stuff
        }
    }
}
```

Commands implements the marker interface `ICommand` and command handlers implements `ICommandHandler<in TCommand>`.

This example uses the dependency `ICultureService` to set the current culture to the given command value.

## Queries

> Queries: Return a result and do not change the observable state of the system (are free of side effects).
>
> — <cite>[Martin Fowler](http://martinfowler.com/bliki/CommandQuerySeparation.html)</cite>

Create a `Query`, `QueryHandler` and `Result`:

```csharp
using System.Threading.Tasks;

namespace CommandQuery.Sample.Queries
{
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

            return await Task.FromResult(result); // TODO: do some real query stuff
        }
    }
}
```

Queries implements the marker interface `IQuery<TResult>` and query handlers implements `IQueryHandler<in TQuery, TResult>`.

This example uses the dependency `IDateTimeProxy` to return a result with the current time.
