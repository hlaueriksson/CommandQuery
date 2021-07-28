using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.Sample.AzureFunctions.V5
{
    public class Query
    {
        private readonly IQueryFunction _queryFunction;

        public Query(IQueryFunction queryFunction)
        {
            _queryFunction = queryFunction;
        }

        [Function("Query")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query/{queryName}")] HttpRequestData req, FunctionContext executionContext, string queryName)
        {
            var logger = executionContext.GetLogger("Query");

            return await _queryFunction.HandleAsync(queryName, req, logger);
        }
    }
}
