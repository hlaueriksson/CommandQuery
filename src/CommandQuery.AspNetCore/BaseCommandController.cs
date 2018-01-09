using System;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.AspNetCore.Internal;
using CommandQuery.Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore
{
    public abstract class BaseCommandController : Controller
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ILogger<BaseCommandController> _logger;

        protected BaseCommandController(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        protected BaseCommandController(ICommandProcessor commandProcessor, ILogger<BaseCommandController> logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Help()
        {
            var baseUrl = Request.GetEncodedUrl();
            var commands = _commandProcessor.GetCommands();

            var result = commands.Select(x => new { command = x.Name, curl = x.GetCurl(baseUrl) });

            return Json(result);
        }

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