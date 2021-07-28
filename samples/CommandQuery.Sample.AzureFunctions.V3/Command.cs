using System.Threading;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Sample.AzureFunctions.V3
{
    public class Command
    {
        private readonly ICommandFunction _commandFunction;

        public Command(ICommandFunction commandFunction)
        {
            _commandFunction = commandFunction;
        }

        [FunctionName("Command")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequest req, ILogger log, CancellationToken cancellationToken, string commandName)
        {
            return await _commandFunction.HandleAsync(commandName, req, log, cancellationToken);
        }
    }
}
