using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace CommandQuery.Sample.AzureFunctions.VsCode2
{
    public static class Command
    {
        private static readonly CommandFunction Func = new CommandFunction(typeof(FooCommand).Assembly.GetCommandProcessor());
        
        [FunctionName("Command")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "command/{commandName}")] HttpRequest req, TraceWriter log, string commandName)
        {
            return await Func.Handle(commandName, req, log);
        }
    }
}