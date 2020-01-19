using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.Internal;

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
            catch (Exception exception)
            {
                _logger?.Error(Request, exception.GetCommandCategory(), exception, "Handle command failed: {CommandName}, {Payload}", commandName, json);

                return exception.IsHandled() ? (IHttpActionResult)BadRequest(exception.Message) : InternalServerError(exception);
            }
        }
    }
}