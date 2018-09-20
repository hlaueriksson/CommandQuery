using System.Web.Http;
using System.Web.Http.Tracing;
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Sample.AspNet.WebApi.Controllers
{
    [RoutePrefix("api/query")]
    public class QueryController : BaseQueryController
    {
        public QueryController(IQueryProcessor queryProcessor, ITraceWriter logger) : base(queryProcessor, logger)
        {
        }
    }
}