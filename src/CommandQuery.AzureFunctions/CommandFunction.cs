using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;

#if NET461
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
#endif

#if NETSTANDARD2_0
using CommandQuery.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
#endif

namespace CommandQuery.AzureFunctions
{
    /// <summary>
    /// Handles commands for the Azure function.
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

#if NET461
        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="req">A <see cref="HttpRequestMessage" /></param>
        /// <param name="log">A <see cref="TraceWriter" /></param>
        /// <returns>200, 400 or 500</returns>
        public async Task<HttpResponseMessage> Handle(string commandName, HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Handle {commandName}");

            try
            {
                var result = await _commandProcessor.ProcessWithOrWithoutResultAsync(commandName, await req.Content.ReadAsStringAsync());

                if (result == CommandResult.None) return req.CreateResponse(HttpStatusCode.OK);

                return req.CreateResponse(HttpStatusCode.OK, result.Value);
            }
            catch (CommandProcessorException exception)
            {
                log.Error("Handle command failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (CommandValidationException exception)
            {
                log.Error("Handle command failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (Exception exception)
            {
                log.Error("Handle command failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, exception.Message);
            }
        }
#endif

#if NETSTANDARD2_0
        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="req">A <see cref="HttpRequest" /></param>
        /// <param name="log">An <see cref="ILogger" /></param>
        /// <returns>200, 400 or 500</returns>
        public async Task<IActionResult> Handle(string commandName, HttpRequest req, ILogger log)
        {
            log.LogInformation($"Handle {commandName}");

            try
            {
                var result = await _commandProcessor.ProcessWithOrWithoutResultAsync(commandName, await req.ReadAsStringAsync());

                if (result == CommandResult.None) return new OkResult();

                return new OkObjectResult(result.Value);
            }
            catch (CommandProcessorException exception)
            {
                log.LogError(exception, "Handle command failed");

                return new BadRequestObjectResult(exception.ToError());
            }
            catch (CommandValidationException exception)
            {
                log.LogError(exception, "Handle command failed");

                return new BadRequestObjectResult(exception.ToError());
            }
            catch (Exception exception)
            {
                log.LogError(exception, "Handle command failed");

                return new ObjectResult(exception.ToError())
                {
                    StatusCode = 500
                };
            }
        }
#endif
    }
}