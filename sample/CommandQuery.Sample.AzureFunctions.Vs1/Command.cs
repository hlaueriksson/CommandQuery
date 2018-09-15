using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Commands;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.AzureFunctions.Vs1
{
    public static class Command
    {
        private static readonly CommandFunction Func = new CommandFunction(typeof(FooCommand).Assembly.GetCommandProcessor(GetContainerBuilder()));

        [FunctionName("Command")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequestMessage req, TraceWriter log, string commandName)
        {
            return await Func.Handle(commandName, req, log);
        }

        private static ContainerBuilder GetContainerBuilder()
        {
            var builder = new ContainerBuilder();
            // Register handler dependencies
            builder.RegisterType<CultureService>().As<ICultureService>();

            return builder;
        }
    }
}