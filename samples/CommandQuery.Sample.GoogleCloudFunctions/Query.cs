using CommandQuery.GoogleCloudFunctions;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    [FunctionsStartup(typeof(Startup))]
    public class Query(IQueryFunction queryFunction) : IHttpFunction
    {
        public async Task HandleAsync(HttpContext context)
        {
            var queryName = context.Request.Path.Value!.Substring("/api/query/".Length);

            await queryFunction.HandleAsync(queryName, context, null, context.RequestAborted);
        }
    }
}
