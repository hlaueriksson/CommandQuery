using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    [ApiController]
    [Route("api/[controller]")]
    internal class CommandWithResultController<TCommand, TResult> : ControllerBase where TCommand : ICommand<TResult>
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ILogger<CommandWithResultController<TCommand, TResult>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandWithResultController&lt;TCommand, TResult&gt;" /> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor" /></param>
        public CommandWithResultController(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandWithResultController&lt;TCommand, TResult&gt;" /> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor" /></param>
        /// <param name="logger">An <see cref="ILogger" /></param>
        public CommandWithResultController(ICommandProcessor commandProcessor, ILogger<CommandWithResultController<TCommand, TResult>> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>The result + 200, 400 or 500</returns>
        [HttpPost]
        public async Task<IActionResult> Handle(TCommand command)
        {
            try
            {
                var result = await _commandProcessor.ProcessWithResultAsync(command);

                return Ok(result);
            }
            catch (CommandProcessorException exception)
            {
                _logger?.LogError(LogEvents.CommandProcessorException, exception, "Handle command failed");

                return BadRequest(exception); // TODO
            }
            catch (CommandValidationException exception)
            {
                _logger?.LogError(LogEvents.CommandValidationException, exception, "Handle command failed");

                return BadRequest(exception);
            }
            catch (Exception exception)
            {
                _logger?.LogError(LogEvents.CommandException, exception, "Handle command failed");

                return StatusCode(500, exception); // InternalServerError
            }
        }
    }
}