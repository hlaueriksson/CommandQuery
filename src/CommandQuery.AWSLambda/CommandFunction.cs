using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.SystemTextJson;

namespace CommandQuery.AWSLambda
{
    /// <summary>
    /// Handles commands for the Lambda function.
    /// </summary>
    public class CommandFunction : ICommandFunction
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

        /// <inheritdoc />
        public async Task<APIGatewayProxyResponse> HandleAsync(string commandName, APIGatewayProxyRequest request, ILambdaLogger logger)
        {
            logger.LogLine($"Handle {commandName}");

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

                return request.Ok(result.Value, _options);
            }
            catch (Exception exception)
            {
                logger.LogLine($"Handle command failed: {commandName}, {request.Body}, {exception.Message}");

                return exception.IsHandled() ? request.BadRequest(exception, _options) : request.InternalServerError(exception, _options);
            }
        }

        /// <inheritdoc />
        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleAsync(string commandName, APIGatewayHttpApiV2ProxyRequest request, ILambdaLogger logger)
        {
            logger.LogLine($"Handle {commandName}");

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                var result = await _commandProcessor.ProcessAsync(commandName, request.Body, _options).ConfigureAwait(false);

                if (result == CommandResult.None)
                {
                    return new APIGatewayHttpApiV2ProxyResponse { StatusCode = (int)HttpStatusCode.OK };
                }

                return request.Ok(result.Value, _options);
            }
            catch (Exception exception)
            {
                logger.LogLine($"Handle command failed: {commandName}, {request.Body}, {exception.Message}");

                return exception.IsHandled() ? request.BadRequest(exception, _options) : request.InternalServerError(exception, _options);
            }
        }
    }
}
