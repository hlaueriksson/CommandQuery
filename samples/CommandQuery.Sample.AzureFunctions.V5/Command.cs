using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.Sample.AzureFunctions.V5
{
    public class Command
    {
        private readonly ICommandFunction _commandFunction;

        public Command(ICommandFunction commandFunction)
        {
            _commandFunction = commandFunction;
        }

        [Function("Command")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequestData req, FunctionContext executionContext, string commandName)
        {
            var logger = executionContext.GetLogger("Command");

            return await _commandFunction.HandleAsync(commandName, req, logger);
        }
    }
}
