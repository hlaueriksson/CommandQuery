using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Sample.AspNet.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class QueryController : BaseQueryController
    {
        protected QueryController(IQueryProcessor queryProcessor) : base(queryProcessor)
        {
        }

        protected QueryController(IQueryProcessor queryProcessor, ITraceWriter logger) : base(queryProcessor, logger)
        {
        }
    }
}