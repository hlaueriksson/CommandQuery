using CommandQuery.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.Sample.AzureFunctions
{
    public class Query(IQueryFunction queryFunction)
    {
        [Function(nameof(Query))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query/{queryName}")] HttpRequestData req, FunctionContext executionContext, string queryName) =>
            await queryFunction.HandleAsync(queryName, req, null, executionContext.CancellationToken);
    }
}
