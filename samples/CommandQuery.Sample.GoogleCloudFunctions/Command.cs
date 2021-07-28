using System.Threading.Tasks;
using CommandQuery.GoogleCloudFunctions;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    [FunctionsStartup(typeof(Startup))]
    public class Command : IHttpFunction
    {
        private readonly ILogger _logger;
        private readonly ICommandFunction _commandFunction;

        public Command(ILogger<Command> logger, ICommandFunction commandFunction)
        {
            _logger = logger;
            _commandFunction = commandFunction;
        }

        public async Task HandleAsync(HttpContext context)
        {
            var commandName = context.Request.Path.Value.Substring("/api/command/".Length);

            await _commandFunction.HandleAsync(commandName, context, _logger, context.RequestAborted);
        }
    }
}
