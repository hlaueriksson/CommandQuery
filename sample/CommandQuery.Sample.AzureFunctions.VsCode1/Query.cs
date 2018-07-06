using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Queries;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.AzureFunctions.VsCode1
{
    public static class Query
    {
        private static readonly QueryFunction Func = new QueryFunction(typeof(BarQuery).Assembly.GetQueryProcessor(GetContainerBuilder()));

        [FunctionName("Query")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "query/{queryName}")] HttpRequestMessage req, TraceWriter log, string queryName)
        {
            return await Func.Handle(queryName, req, log);
        }

        private static ContainerBuilder GetContainerBuilder()
        {
            var builder = new ContainerBuilder();
            // Register handler dependencies
            builder.RegisterType<DateTimeProxy>().As<IDateTimeProxy>();

            return builder;
        }
    }
}