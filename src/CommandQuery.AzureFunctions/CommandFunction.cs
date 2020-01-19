using System;
using System.Threading.Tasks;
using CommandQuery.Internal;

#if NET461
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host;
#endif

#if NETSTANDARD2_0 || NETCOREAPP3_0
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
    public interface ICommandFunction
    {
#if NET461
        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="req">A <see cref="HttpRequestMessage" /></param>
        /// <param name="log">A <see cref="TraceWriter" /></param>
        /// <returns>200, 400 or 500</returns>
        Task<HttpResponseMessage> Handle(string commandName, HttpRequestMessage req, TraceWriter log);
#endif

#if NETSTANDARD2_0 || NETCOREAPP3_0
        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="req">A <see cref="HttpRequest" /></param>
        /// <param name="log">An <see cref="ILogger" /></param>
        /// <returns>200, 400 or 500</returns>
        Task<IActionResult> Handle(string commandName, HttpRequest req, ILogger log);
#endif
    }

    /// <summary>
    /// Handles commands for the Azure function.
    /// </summary>
    public class CommandFunction : ICommandFunction
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
            catch (Exception exception)
            {
                var payload = await req.Content.ReadAsStringAsync();
                log.Error($"Handle command failed: {commandName}, {payload}", exception);

                return req.CreateErrorResponse(exception.IsHandled() ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError, exception.Message, exception);
            }
        }
#endif

#if NETSTANDARD2_0 || NETCOREAPP3_0
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
            catch (Exception exception)
            {
                var payload = await req.ReadAsStringAsync();
                log.LogError(exception.GetCommandEventId(), exception, "Handle command failed: {CommandName}, {Payload}", commandName, payload);

                return exception.IsHandled() ? new BadRequestObjectResult(exception.ToError()) : new ObjectResult(exception.ToError()) { StatusCode = 500 };
            }
        }
#endif
    }
}