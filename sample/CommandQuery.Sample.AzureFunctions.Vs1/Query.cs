using System.Net.Http;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Queries;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.AzureFunctions.Vs1
{
    public static class Query
    {
        private static readonly QueryFunction Func = new QueryFunction(typeof(BarQuery).Assembly.GetQueryProcessor());

        [FunctionName("Query")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "query/{queryName}")] HttpRequestMessage req, TraceWriter log, string queryName)
        {
            return await Func.Handle(queryName, req, log);
        }
    }
}