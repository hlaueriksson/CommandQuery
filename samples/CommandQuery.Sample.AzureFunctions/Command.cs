using CommandQuery.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.Sample.AzureFunctions
{
    public class Command(ICommandFunction commandFunction)
    {
        [Function(nameof(Command))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequestData req, FunctionContext executionContext, string commandName) =>
            await commandFunction.HandleAsync(commandName, req, null, executionContext.CancellationToken);
    }
}
