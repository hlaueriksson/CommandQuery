using System;
using System.Threading.Tasks;
using CommandQuery.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AzureFunctions
{
    /// <summary>
    /// Handles commands for the Azure function.
    /// </summary>
    public interface ICommandFunction
    {
        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="req">A <see cref="HttpRequest"/></param>
        /// <param name="log">An <see cref="ILogger"/></param>
        /// <returns>200, 400 or 500</returns>
        Task<IActionResult> Handle(string commandName, HttpRequest req, ILogger log);
    }

    /// <summary>
    /// Handles commands for the Azure function.
    /// </summary>
    public class CommandFunction : ICommandFunction
    {
        private readonly ICommandProcessor _commandProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFunction"/> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor"/></param>
        public CommandFunction(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="req">A <see cref="HttpRequest"/></param>
        /// <param name="log">An <see cref="ILogger"/></param>
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
    }
}
