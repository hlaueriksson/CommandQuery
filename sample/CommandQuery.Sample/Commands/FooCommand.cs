using System.Threading.Tasks;

namespace CommandQuery.Sample.Commands
{
    public class FooCommand : ICommand
    {
        public string Value { get; set; }
    }

    public class FooCommandHandler : ICommandHandler<FooCommand>
    {
        public async Task HandleAsync(FooCommand command)
        {
            // TODO: do some real command stuff

            await Task.Delay(10);
        }
    }
}