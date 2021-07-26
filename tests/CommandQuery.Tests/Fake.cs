using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CommandQuery.Tests
{
    public class FakeCommand : ICommand
    {
    }

    public class FakeCommandHandler : ICommandHandler<FakeCommand>
    {
        public Action<FakeCommand> Callback { get; set; }

        public async Task HandleAsync(FakeCommand command, CancellationToken cancellationToken)
        {
            Callback(command);

            await Task.CompletedTask;
        }
    }

    public class FakeResultCommand : ICommand<FakeResult>
    {
    }

    public class FakeResultCommandHandler : ICommandHandler<FakeResultCommand, FakeResult>
    {
        public Func<FakeResultCommand, FakeResult> Callback { get; set; }

        public async Task<FakeResult> HandleAsync(FakeResultCommand command, CancellationToken cancellationToken)
        {
            var result = Callback(command);

            return await Task.FromResult(result);
        }
    }

    public class FakeQuery : IQuery<FakeResult>
    {
    }

    public class FakeResult
    {
    }

    public class FakeQueryHandler : IQueryHandler<FakeQuery, FakeResult>
    {
        public Func<FakeQuery, FakeResult> Callback { get; set; }

        public async Task<FakeResult> HandleAsync(FakeQuery query, CancellationToken cancellationToken)
        {
            var result = Callback(query);

            return await Task.FromResult(result);
        }
    }

    // https://github.com/dotnet/runtime/tree/main/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/Value
    // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-supported-collection-types
    public class FakeComplexQuery : IQuery<IEnumerable<FakeResult>>
    {
        public bool Boolean { get; set; }
        public byte Byte { get; set; }
        public char Char { get; set; }
        public DateTime DateTime { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public DayOfWeek Enum { get; set; }
        public Guid Guid { get; set; }
        public short Int16 { get; set; }
        public int Int32 { get; set; }
        public long Int64 { get; set; }
        public sbyte SByte { get; set; }
        public float Single { get; set; }
        public string String { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public ushort UInt16 { get; set; }
        public uint UInt32 { get; set; }
        public ulong UInt64 { get; set; }
        public Uri Uri { get; set; }
        public Version Version { get; set; }

        public int? Nullable { get; set; }

        public int[] Array { get; set; }
        public IEnumerable<int> IEnumerable { get; set; }
        public IList<int> IList { get; set; }
        public IReadOnlyList<int> IReadOnlyList { get; set; }
    }

    public class FakeComplexQueryHandler : IQueryHandler<FakeComplexQuery, IEnumerable<FakeResult>>
    {
        public async Task<IEnumerable<FakeResult>> HandleAsync(FakeComplexQuery query, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new []{ new FakeResult() });
        }
    }

    public class FakeDateTimeQuery : IQuery<FakeResult>
    {
        public DateTime DateTimeUnspecified { get; set; }
        public DateTime DateTimeUtc { get; set; }
        public DateTime DateTimeLocal { get; set; }
        public DateTime[] DateTimeArray { get; set; }
    }

    public class FakeNestedQuery : IQuery<FakeResult>
    {
        public string Foo { get; set; }

        public FakeNestedChild Child { get; set; }
    }

    public class FakeNestedChild
    {
        public string Foo { get; set; }

        public FakeNestedGrandchild Child { get; set; }
    }

    public class FakeNestedGrandchild
    {
        public string Foo { get; set; }
    }

    public class FakeMultiCommand1 : ICommand { }
    public class FakeMultiCommand2 : ICommand { }
    public class FakeMultiResultCommand1 : ICommand<FakeResult> { }
    public class FakeMultiResultCommand2 : ICommand<FakeResult> { }
    public class FakeMultiQuery1 : IQuery<FakeResult> { }
    public class FakeMultiQuery2 : IQuery<FakeResult> { }
    public class FakeMultiHandler :
        ICommandHandler<FakeMultiCommand1>,
        ICommandHandler<FakeMultiCommand2>,
        ICommandHandler<FakeMultiResultCommand1, FakeResult>,
        ICommandHandler<FakeMultiResultCommand2, FakeResult>,
        IQueryHandler<FakeMultiQuery1, FakeResult>,
        IQueryHandler<FakeMultiQuery2, FakeResult>
    {

        public Task HandleAsync(FakeMultiCommand1 command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task HandleAsync(FakeMultiCommand2 command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FakeResult> HandleAsync(FakeMultiResultCommand1 command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FakeResult> HandleAsync(FakeMultiResultCommand2 command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FakeResult> HandleAsync(FakeMultiQuery1 query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FakeResult> HandleAsync(FakeMultiQuery2 query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeError : IError
    {
        public string Message { get; set; }

        public Dictionary<string, object> Details { get; set; }
    }
}
