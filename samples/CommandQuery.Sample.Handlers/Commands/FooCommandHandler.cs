using System.Threading.Tasks;
using CommandQuery.Sample.Contracts.Commands;

namespace CommandQuery.Sample.Handlers.Commands
{
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