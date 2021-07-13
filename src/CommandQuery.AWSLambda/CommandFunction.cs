using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.SystemTextJson;

namespace CommandQuery.AWSLambda
{
    /// <summary>
    /// Handles commands for the Lambda function.
    /// </summary>
    public class CommandFunction
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly JsonSerializerOptions? _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFunction"/> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor"/>.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during deserialization of <see cref="APIGatewayProxyRequest.Body"/> and serialization of <see cref="APIGatewayProxyResponse.Body"/>.</param>
        public CommandFunction(ICommandProcessor commandProcessor, JsonSerializerOptions? options = null)
        {
            _commandProcessor = commandProcessor;
            _options = options;
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="request">An <see cref="APIGatewayProxyRequest"/>.</param>
        /// <param name="logger">An <see cref="ILambdaLogger"/>.</param>
        /// <returns>The result for status code <c>200</c>, or an error for status code <c>400</c> and <c>500</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public async Task<APIGatewayProxyResponse> HandleAsync(string commandName, APIGatewayProxyRequest request, ILambdaLogger? logger)
        {
            logger?.LogLine($"Handle {commandName}");

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                var result = await _commandProcessor.ProcessAsync(commandName, request.Body, _options).ConfigureAwait(false);

                if (result == CommandResult.None)
                {
                    return new APIGatewayProxyResponse { StatusCode = (int)HttpStatusCode.OK };
                }

                return result.Value.Ok(_options);
            }
            catch (Exception exception)
            {
                logger?.LogLine($"Handle command failed: {commandName}, {request.Body}, {exception.Message}");

                return exception.IsHandled() ? exception.BadRequest(_options) : exception.InternalServerError(_options);
            }
        }
    }
}
