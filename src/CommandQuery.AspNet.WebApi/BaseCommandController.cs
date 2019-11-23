using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi.Internal;
using CommandQuery.Exceptions;

namespace CommandQuery.AspNet.WebApi
{
    /// <summary>
    /// Base class for command controllers.
    /// </summary>
    public abstract class BaseCommandController : ApiController
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly ITraceWriter _logger;

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
        /// <param name="logger">An <see cref="ITraceWriter" /></param>
        protected BaseCommandController(ICommandProcessor commandProcessor, ITraceWriter logger)
        {
            _commandProcessor = commandProcessor;
            _logger = logger;
        }

        /// <summary>
        /// Gets help.
        /// </summary>
        /// <returns>Command help</returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Help()
        {
            var baseUrl = Request.RequestUri.ToString();
            var commands = _commandProcessor.GetCommandTypes();

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
        public async Task<IHttpActionResult> Handle(string commandName, [FromBody] Newtonsoft.Json.Linq.JObject json)
        {
            try
            {
                var result = await _commandProcessor.ProcessWithOrWithoutResultAsync(commandName, json);

                if (result == CommandResult.None) return Ok();

                return Ok(result.Value);
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