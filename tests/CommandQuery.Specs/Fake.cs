using System.Threading.Tasks;

namespace CommandQuery.Specs
{
    public class FakeCommand : ICommand
    {
    }

    public class FakeCommandHandler : ICommandHandler<FakeCommand>
    {
        public async Task HandleAsync(FakeCommand command)
        {
            await Task.CompletedTask;
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
        public async Task<FakeResult> HandleAsync(FakeQuery query)
        {
            return await Task.FromResult(new FakeResult());
        }
    }
}