using System;
using System.Threading.Tasks;
using CommandQuery.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    [ApiController]
    [Route("api/command/[controller]")]
    internal class CommandController<TCommand> : ControllerBase
        where TCommand : ICommand
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandController{TCommand}"/> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor"/>.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public CommandController(ICommandProcessor commandProcessor, ILogger<CommandController<TCommand>> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>200, 400 or 500.</returns>
        [HttpPost]
        public async Task<IActionResult> HandleAsync(TCommand command)
        {
            _logger?.LogInformation("Handle {@Command}", command);

            try
            {
                await _commandProcessor.ProcessAsync(command).ConfigureAwait(false);

                return Ok();
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Handle command failed: {@Command}", command);

                return exception.IsHandled() ? BadRequest(exception.ToError()) : StatusCode(500, exception.ToError());
            }
        }
    }
}
