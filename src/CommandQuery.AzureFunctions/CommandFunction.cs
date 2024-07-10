using System.Net;
using System.Text.Json;
using CommandQuery.SystemTextJson;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AzureFunctions
{
    /// <inheritdoc />
    public class CommandFunction : ICommandFunction
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly JsonSerializerOptions? _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFunction"/> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor"/>.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during deserialization of <see cref="HttpRequestData.Body"/> and serialization of <see cref="HttpResponseData.Body"/>.</param>
        public CommandFunction(ICommandProcessor commandProcessor, JsonSerializerOptions? options = null)
        {
            _commandProcessor = commandProcessor;
            _options = options;
        }

        /// <inheritdoc />
        public async Task<HttpResponseData> HandleAsync(string commandName, HttpRequestData req, ILogger? logger, CancellationToken cancellationToken = default)
        {
            logger?.LogInformation("Handle {Command}", commandName);

            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            try
            {
                var result = await _commandProcessor.ProcessAsync(commandName, await req.ReadAsStringAsync().ConfigureAwait(false), _options, cancellationToken).ConfigureAwait(false);

                if (result == CommandResult.None)
                {
                    return req.CreateResponse(HttpStatusCode.OK);
                }

                return await req.OkAsync(result.Value, _options).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var payload = await req.ReadAsStringAsync().ConfigureAwait(false);
                logger?.LogError(exception, "Handle command failed: {Command}, {Payload}", commandName, payload);

                return exception.IsHandled()
                    ? await req.BadRequestAsync(exception, _options).ConfigureAwait(false)
                    : await req.InternalServerErrorAsync(exception, _options).ConfigureAwait(false);
            }
        }
    }
}
