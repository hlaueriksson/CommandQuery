using System;
using System.Text.Json;
using System.Threading.Tasks;
using CommandQuery.SystemTextJson;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.GoogleCloudFunctions
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
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during deserialization of <see cref="HttpRequest.Body"/> and serialization of <see cref="HttpResponse.Body"/>.</param>
        public CommandFunction(ICommandProcessor commandProcessor, JsonSerializerOptions? options = null)
        {
            _commandProcessor = commandProcessor;
            _options = options;
        }

        /// <inheritdoc />
        public async Task HandleAsync(string commandName, HttpContext context, ILogger? logger)
        {
            logger?.LogInformation("Handle {Command}", commandName);

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            try
            {
                var result = await _commandProcessor.ProcessAsync(commandName, await context.Request.ReadAsStringAsync().ConfigureAwait(false), _options).ConfigureAwait(false);

                context.Response.StatusCode = StatusCodes.Status200OK;

                if (result == CommandResult.None)
                {
                    return;
                }

                await context.Response.OkAsync(result.Value, _options).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var payload = await context.Request.ReadAsStringAsync().ConfigureAwait(false);
                logger?.LogError(exception, "Handle command failed: {Command}, {Payload}", commandName, payload);

                if (exception.IsHandled())
                {
                    await context.Response.BadRequestAsync(exception).ConfigureAwait(false);
                    return;
                }

                await context.Response.InternalServerErrorAsync(exception).ConfigureAwait(false);
            }
        }
    }
}
