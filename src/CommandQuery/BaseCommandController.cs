using System;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery
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
        public async Task<object> Handle(string commandName, [FromBody] Newtonsoft.Json.Linq.JObject json)
        {
            try
            {
                await _commandProcessor.ProcessAsync(commandName, json);

                return string.Empty;
            }
            catch (CommandProcessorException exception)
            {
                _logger?.LogError(LogEvents.CommandProcessorException, exception, "Handle command failed");

                Response.StatusCode = 400; // BadRequest

                return exception.Message;
            }
            catch (CommandValidationException exception)
            {
                _logger?.LogError(LogEvents.CommandValidationException, exception, "Handle command failed");

                Response.StatusCode = 400; // BadRequest

                return "Validation error: " + exception.Message;
            }
            catch (Exception exception)
            {
                _logger?.LogError(LogEvents.CommandException, exception, "Handle command failed");

                Response.StatusCode = 500; // InternalServerError

                return "Error: " + exception.Message;
            }
        }
    }
}