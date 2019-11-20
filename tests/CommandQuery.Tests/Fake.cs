using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandQuery.Tests
{
    public class FakeCommand : ICommand
    {
    }

    public class FakeCommandHandler : ICommandHandler<FakeCommand>
    {
        private readonly Action<FakeCommand> _callback;

        public FakeCommandHandler(Action<FakeCommand> callback)
        {
            _callback = callback;
        }

        public async Task HandleAsync(FakeCommand command)
        {
            _callback(command);

            await Task.CompletedTask;
        }
    }

    public class FakeResultCommand : ICommand<FakeResult>
    {
    }

    public class FakeResultCommandHandler : ICommandHandler<FakeResultCommand, FakeResult>
    {
        private readonly Func<FakeResultCommand, FakeResult> _callback;

        public FakeResultCommandHandler(Func<FakeResultCommand, FakeResult> callback)
        {
            _callback = callback;
        }

        public async Task<FakeResult> HandleAsync(FakeResultCommand resultCommand)
        {
            var result = _callback(resultCommand);

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
        private readonly Func<FakeQuery, FakeResult> _callback;

        public FakeQueryHandler(Func<FakeQuery, FakeResult> callback)
        {
            _callback = callback;
        }

        public async Task<FakeResult> HandleAsync(FakeQuery query)
        {
            var result = _callback(query);

            return await Task.FromResult(result);
        }
    }

    public class FakeComplexQuery : IQuery<IEnumerable<FakeResult>>
    {
        public string String { get; set; }
        public int Int { get; set; }
        public bool Bool { get; set; }
        public DateTime DateTime { get; set; }
        public Guid Guid { get; set; }
        public double? NullableDouble { get; set; }
        public int[] Array { get; set; }
        public IEnumerable<int> IEnumerable { get; set; }
        public List<int> List { get; set; }
    }

    public class FakeComplexQueryHandler : IQueryHandler<FakeComplexQuery, IEnumerable<FakeResult>>
    {
        private readonly Func<FakeComplexQuery, IEnumerable<FakeResult>> _callback;

        public FakeComplexQueryHandler(Func<FakeComplexQuery, IEnumerable<FakeResult>> callback)
        {
            _callback = callback;
        }

        public async Task<IEnumerable<FakeResult>> HandleAsync(FakeComplexQuery query)
        {
            var result = _callback(query);

            return await Task.FromResult(result);
        }
    }
}