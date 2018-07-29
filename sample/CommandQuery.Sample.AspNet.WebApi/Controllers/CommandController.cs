using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Sample.AspNet.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class CommandController : BaseCommandController
    {
        protected CommandController(ICommandProcessor commandProcessor) : base(commandProcessor)
        {
        }

        protected CommandController(ICommandProcessor commandProcessor, ITraceWriter logger) : base(commandProcessor, logger)
        {
        }
    }
}