using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Sample.AspNet.WebApi.Controllers
{
    [RoutePrefix("api/command")]
    public class CommandController : BaseCommandController
    {
        public CommandController(ICommandProcessor commandProcessor, ITraceWriter logger) : base(commandProcessor, logger)
        {
        }
    }
}