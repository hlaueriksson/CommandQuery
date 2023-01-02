using CommandQuery.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Sample.AzureFunctions.V6
{
    public class Query
    {
        private readonly IQueryFunction _queryFunction;
        private readonly ILogger _logger;

        public Query(IQueryFunction queryFunction, ILoggerFactory loggerFactory)
        {
            _queryFunction = queryFunction;
            _logger = loggerFactory.CreateLogger<Query>();
        }

        [Function("Query")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query/{queryName}")] HttpRequestData req, FunctionContext executionContext, string queryName) =>
            await _queryFunction.HandleAsync(queryName, req, _logger);
    }
}
