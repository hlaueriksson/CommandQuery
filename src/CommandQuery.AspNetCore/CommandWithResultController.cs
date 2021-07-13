using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    [ApiController]
    [Route("api/command/[controller]")]
    internal class CommandController<TCommand, TResult> : ControllerBase
        where TCommand : ICommand<TResult>
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandController{TCommand,TResult}"/> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor"/>.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        public CommandController(ICommandProcessor commandProcessor, ILogger<CommandController<TCommand, TResult>> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The result + 200, 400 or 500.</returns>
        [HttpPost]
        public async Task<IActionResult> HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("Handle {@Command}", command);

            try
            {
                var result = await _commandProcessor.ProcessAsync(command, cancellationToken).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Handle command failed: {@Command}", command);

                return exception.IsHandled() ? BadRequest(exception.ToError()) : StatusCode(500, exception.ToError());
            }
        }
    }
}
