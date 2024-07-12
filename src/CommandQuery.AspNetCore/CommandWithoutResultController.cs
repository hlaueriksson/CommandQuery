using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    [ApiController]
    [Route("api/command/[controller]")]
#pragma warning disable SA1649 // File name should match first type name
    internal class CommandController<TCommand> : ControllerBase
#pragma warning restore SA1649 // File name should match first type name
        where TCommand : ICommand
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ILogger _logger;

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
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The result for status code <c>200</c>, or an error for status code <c>400</c> and <c>500</c>.</returns>
        [HttpPost]
        public async Task<IActionResult> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handle {@Command}", command);

            try
            {
                await _commandProcessor.ProcessAsync(command, cancellationToken).ConfigureAwait(false);

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Handle command failed: {@Command}", command);

                return exception.IsHandled() ? BadRequest(exception.ToError()) : StatusCode(500, exception.ToError());
            }
        }
    }
}
