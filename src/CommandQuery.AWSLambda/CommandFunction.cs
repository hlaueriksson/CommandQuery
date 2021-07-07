using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda.Internal;
using CommandQuery.Internal;
using CommandQuery.SystemTextJson;

namespace CommandQuery.AWSLambda
{
    /// <summary>
    /// Handles commands for the Lambda function.
    /// </summary>
    public class CommandFunction
    {
        private readonly ICommandProcessor _commandProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFunction"/> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor"/>.</param>
        public CommandFunction(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="request">An <see cref="APIGatewayProxyRequest"/>.</param>
        /// <param name="logger">An <see cref="ILambdaLogger"/>.</param>
        /// <returns>200, 400 or 500.</returns>
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
                var result = await _commandProcessor.ProcessWithOrWithoutResultAsync(commandName, request.Body).ConfigureAwait(false);

                if (result == CommandResult.None)
                {
                    return new APIGatewayProxyResponse { StatusCode = (int)HttpStatusCode.OK };
                }

                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonSerializer.Serialize(result.Value),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                };
            }
            catch (Exception exception)
            {
                logger?.LogLine($"Handle command failed: {commandName}, {request.Body}, {exception.Message}");

                return exception.IsHandled() ? exception.ToBadRequest() : exception.ToInternalServerError();
            }
        }
    }
}
