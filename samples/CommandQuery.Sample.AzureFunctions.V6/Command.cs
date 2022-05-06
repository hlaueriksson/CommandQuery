using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Sample.AzureFunctions.V6
{
    public class Command
    {
        private readonly ICommandFunction _commandFunction;
        private readonly ILogger _logger;

        public Command(ICommandFunction commandFunction, ILoggerFactory loggerFactory)
        {
            _commandFunction = commandFunction;
            _logger = loggerFactory.CreateLogger<Command>();
        }

        [Function("Command")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequestData req, FunctionContext executionContext, string commandName) =>
            await _commandFunction.HandleAsync(commandName, req, _logger);
    }
}
