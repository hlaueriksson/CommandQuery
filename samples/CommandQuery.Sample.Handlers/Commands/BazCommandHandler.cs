using System.Threading.Tasks;
using CommandQuery.Sample.Contracts.Commands;

namespace CommandQuery.Sample.Handlers.Commands
{
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
                // TODO: do some real log stuff
            }

            return await Task.FromResult(result);
        }
    }
}