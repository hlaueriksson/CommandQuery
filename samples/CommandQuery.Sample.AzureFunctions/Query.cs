using CommandQuery.AzureFunctions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CommandQuery.Sample.AzureFunctions
{
    public class Query(IQueryFunction queryFunction)
    {
        [Function(nameof(Query))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query/{queryName}")] HttpRequest req,
            FunctionContext context,
            string queryName) =>
            await queryFunction.HandleAsync(queryName, req, context.CancellationToken);
    }
}
