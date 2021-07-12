#if NETCOREAPP3_1
using System;
using System.Threading.Tasks;
using CommandQuery.NewtonsoftJson;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CommandQuery.AzureFunctions
{
    /// <inheritdoc />
    public class CommandFunction : ICommandFunction
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly JsonSerializerSettings? _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFunction"/> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor"/>.</param>
        /// <param name="settings"><see cref="JsonSerializerSettings"/> to control the behavior during deserialization of <see cref="HttpRequest.Body"/>.</param>
        public CommandFunction(ICommandProcessor commandProcessor, JsonSerializerSettings? settings = null)
        {
            _commandProcessor = commandProcessor;
            _settings = settings;
        }

        /// <inheritdoc />
        public async Task<IActionResult> HandleAsync(string commandName, HttpRequest req, ILogger? logger)
        {
            logger?.LogInformation("Handle {Command}", commandName);

            try
            {
                var result = await _commandProcessor.ProcessAsync(commandName, await req.ReadAsStringAsync().ConfigureAwait(false), _settings).ConfigureAwait(false);

                if (result == CommandResult.None)
                {
                    return new OkResult();
                }

                return new OkObjectResult(result.Value);
            }
            catch (Exception exception)
            {
                var payload = await req.ReadAsStringAsync().ConfigureAwait(false);
                logger?.LogError(exception, "Handle command failed: {Command}, {Payload}", commandName, payload);

                return exception.IsHandled() ? new BadRequestObjectResult(exception.ToError()) : new ObjectResult(exception.ToError()) { StatusCode = 500 };
            }
        }
    }
}
#endif
