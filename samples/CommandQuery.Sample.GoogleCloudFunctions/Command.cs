using CommandQuery.GoogleCloudFunctions;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.Sample.GoogleCloudFunctions
{
    [FunctionsStartup(typeof(Startup))]
    public class Command(ICommandFunction commandFunction) : IHttpFunction
    {
        public async Task HandleAsync(HttpContext context)
        {
            var commandName = context.Request.Path.Value!.Substring("/api/command/".Length);
            await commandFunction.HandleAsync(commandName, context, context.RequestAborted);
        }
    }
}
