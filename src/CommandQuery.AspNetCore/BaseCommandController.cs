using System;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.AspNetCore.Internal;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// Base class for command controllers.
    /// </summary>
    public abstract class BaseCommandController : Controller
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ILogger<BaseCommandController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommandController" /> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor" /></param>
        protected BaseCommandController(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCommandController" /> class.
        /// </summary>
        /// <param name="commandProcessor">An <see cref="ICommandProcessor" /></param>
        /// <param name="logger">An <see cref="ILogger" /></param>
        protected BaseCommandController(ICommandProcessor commandProcessor, ILogger<BaseCommandController> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Gets help.
        /// </summary>
        /// <returns>Command help</returns>
        [HttpGet]
        public IActionResult Help()
        {
            var baseUrl = Request.GetEncodedUrl();
            var commands = _commandProcessor.GetCommands();

            var result = commands.Select(x => new { command = x.Name, curl = x.GetCurl(baseUrl) });

            return Json(result);
        }

        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>200, 400 or 500</returns>
        [HttpPost]
        [Route("{commandName}")]
        public async Task<IActionResult> Handle(string commandName, [FromBody] Newtonsoft.Json.Linq.JObject json)
        {
            try
            {
                await _commandProcessor.ProcessAsync(commandName, json);

                return Ok();
            }
            catch (CommandProcessorException exception)
            {
                _logger?.LogError(LogEvents.CommandProcessorException, exception, "Handle command failed");

                return BadRequest(exception.ToError());
            }
            catch (CommandValidationException exception)
            {
                _logger?.LogError(LogEvents.CommandValidationException, exception, "Handle command failed");

                return BadRequest(exception.ToError());
            }
            catch (Exception exception)
            {
                _logger?.LogError(LogEvents.CommandException, exception, "Handle command failed");

                return StatusCode(500, exception.ToError()); // InternalServerError
            }
        }
    }
}