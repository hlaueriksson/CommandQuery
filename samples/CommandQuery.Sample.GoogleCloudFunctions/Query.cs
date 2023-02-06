using System.Threading.Tasks;
using CommandQuery.GoogleCloudFunctions;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    [FunctionsStartup(typeof(Startup))]
    public class Query : IHttpFunction
    {
        private readonly ILogger _logger;
        private readonly IQueryFunction _queryFunction;

        public Query(ILogger<Query> logger, IQueryFunction queryFunction)
        {
            _logger = logger;
            _queryFunction = queryFunction;
        }

        public async Task HandleAsync(HttpContext context)
        {
            var queryName = context.Request.Path.Value!.Substring("/api/query/".Length);

            await _queryFunction.HandleAsync(queryName, context, _logger, context.RequestAborted);
        }
    }
}
