using System;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda.Internal;
using CommandQuery.Exceptions;

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
        /// <param name="content">The JSON representation of the command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task Handle(string commandName, string content)
        {
            await _commandProcessor.ProcessAsync(commandName, content);
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
                await Handle(commandName, request.Body);

                return new APIGatewayProxyResponse { StatusCode = (int)HttpStatusCode.OK };
            }
            catch (CommandProcessorException exception)
            {
                context.Logger.LogLine("Handle command failed: " + exception);

                return exception.ToBadRequest();
            }
            catch (CommandValidationException exception)
            {
                context.Logger.LogLine("Handle command failed: " + exception);

                return exception.ToBadRequest();
            }
            catch (Exception exception)
            {
                context.Logger.LogLine("Handle command failed: " + exception);

                return exception.ToInternalServerError();
            }
        }
    }
}