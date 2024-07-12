using CommandQuery.AzureFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CommandQuery.Sample.AzureFunctions
{
    public class Command(ICommandFunction commandFunction)
    {
        [Function(nameof(Command))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequest req,
            FunctionContext context,
            string commandName) =>
            await commandFunction.HandleAsync(commandName, req, context.CancellationToken);
    }
}
