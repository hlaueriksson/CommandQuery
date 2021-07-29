using System.Threading.Tasks;
using CommandQuery.Sample.AspNet.WebApi.Contracts.Commands;

namespace CommandQuery.Sample.AspNet.WebApi.Handlers.Commands
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
            if (command.Value == null) throw new FooCommandException("Value cannot be null", 1337, "Try setting the value to 'en-US'");

            _cultureService.SetCurrentCulture(command.Value);

            await Task.CompletedTask;
        }
    }
}
