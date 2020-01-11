using System;
using System.Threading.Tasks;
using CommandQuery.AspNetCore.Internal;
using CommandQuery.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    [ApiController]
    [Route("api/command/[controller]")]
    internal class CommandController<TCommand> : ControllerBase where TCommand : ICommand
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ILogger<CommandController<TCommand>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandController&lt;TCommand&gt;" /> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor" /></param>
        /// <param name="logger">An <see cref="ILogger" /></param>
        public CommandController(ICommandProcessor commandProcessor, ILogger<CommandController<TCommand>> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>200, 400 or 500</returns>
        [HttpPost]
        public async Task<IActionResult> Handle(TCommand command)
        {
            try
            {
                await _commandProcessor.ProcessAsync(command);

                return Ok();
            }
            catch (CommandProcessorException exception)
            {
                _logger?.LogError(LogEvents.CommandProcessorException, exception, "Handle command failed");

                return BadRequest(exception.ToError());
            }
            catch (CommandException exception)
            {
                _logger?.LogError(LogEvents.CommandException, exception, "Handle command failed");

                return BadRequest(exception.ToError());
            }
            catch (Exception exception)
            {
                _logger?.LogError(LogEvents.UnhandledCommandException, exception, "Handle command failed");

                return StatusCode(500, exception.ToError()); // InternalServerError
            }
        }
    }
}