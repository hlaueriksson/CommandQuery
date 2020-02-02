using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda.Internal;
using CommandQuery.Internal;
using Newtonsoft.Json;

namespace CommandQuery.AWSLambda
{
    /// <summary>
    /// Handles commands for the Lambda function.
    /// </summary>
    public class CommandFunction
    {
        private readonly ICommandProcessor _commandProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFunction" /> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor" /></param>
        public CommandFunction(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="request">An <see cref="APIGatewayProxyRequest" /></param>
        /// <param name="context">An <see cref="ILambdaContext" /></param>
        /// <returns>200, 400 or 500</returns>
        public async Task<APIGatewayProxyResponse> Handle(string commandName, APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine($"Handle {commandName}");

            try
            {
                var result = await _commandProcessor.ProcessWithOrWithoutResultAsync(commandName, request.Body);

                if (result == CommandResult.None) return new APIGatewayProxyResponse { StatusCode = (int)HttpStatusCode.OK };

                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonConvert.SerializeObject(result.Value),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (Exception exception)
            {
                context.Logger.LogLine($"Handle command failed: {commandName}, {request.Body}, {exception.Message}");

                return exception.IsHandled() ? exception.ToBadRequest() : exception.ToInternalServerError();
            }
        }
    }
}