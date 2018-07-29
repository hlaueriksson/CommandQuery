using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi.Internal;
using CommandQuery.Exceptions;

namespace CommandQuery.AspNet.WebApi
{
    public class BaseCommandController : ApiController
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ITraceWriter _logger;

        protected BaseCommandController(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        protected BaseCommandController(ICommandProcessor commandProcessor, ITraceWriter logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        [HttpGet]
        public IHttpActionResult Help()
        {
            var baseUrl = Request.RequestUri.ToString();
            var commands = _commandProcessor.GetCommands();

            var result = commands.Select(x => new { command = x.Name, curl = x.GetCurl(baseUrl) });

            return Json(result);
        }

        [HttpPost]
        [Route("{commandName}")]
        public async Task<IHttpActionResult> Handle(string commandName, [FromBody] Newtonsoft.Json.Linq.JObject json)
        {
            try
            {
                await _commandProcessor.ProcessAsync(commandName, json);

                return Ok();
            }
            catch (CommandProcessorException exception)
            {
                _logger?.Error(Request, LogEvents.CommandProcessorException, exception, "Handle command failed");

                return BadRequest(exception.Message);
            }
            catch (CommandValidationException exception)
            {
                _logger?.Error(Request, LogEvents.CommandValidationException, exception, "Handle command failed");

                return BadRequest(exception.Message);
            }
            catch (Exception exception)
            {
                _logger?.Error(Request, LogEvents.CommandException, exception, "Handle command failed");

                return InternalServerError(exception);
            }
        }
    }
}