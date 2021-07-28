#if NETCOREAPP3_1
using System;
using System.Threading;
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
        /// <param name="settings"><see cref="JsonSerializerSettings"/> to control the behavior during deserialization of <see cref="HttpRequest.Body"/> and serialization of <see cref="HttpResponse.Body"/>.</param>
        public CommandFunction(ICommandProcessor commandProcessor, JsonSerializerSettings? settings = null)
        {
            _commandProcessor = commandProcessor;
            _settings = settings;
        }

        /// <inheritdoc />
        public async Task<IActionResult> HandleAsync(string commandName, HttpRequest req, ILogger? logger, CancellationToken cancellationToken = default)
        {
            logger?.LogInformation("Handle {Command}", commandName);

            try
            {
                var result = await _commandProcessor.ProcessAsync(commandName, await req.ReadAsStringAsync().ConfigureAwait(false), _settings, cancellationToken).ConfigureAwait(false);

                if (result == CommandResult.None)
                {
                    return new OkResult();
                }

                return result.Value.Ok(_settings);
            }
            catch (Exception exception)
            {
                var payload = await req.ReadAsStringAsync().ConfigureAwait(false);
                logger?.LogError(exception, "Handle command failed: {Command}, {Payload}", commandName, payload);

                return exception.IsHandled() ? exception.BadRequest(_settings) : exception.InternalServerError(_settings);
            }
        }
    }
}
#endif
