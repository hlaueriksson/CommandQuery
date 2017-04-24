#r "CommandQuery.dll"
#r "CommandQuery.AzureFunctions.dll"
#r "CommandQuery.Sample.dll"

using System.Net;
using System.Reflection;
using CommandQuery.AzureFunctions;

static CommandFunction func = new CommandFunction(Assembly.Load("CommandQuery.Sample").GetCommandProcessor());

public static async Task<HttpResponseMessage> Run(string commandName, HttpRequestMessage req, TraceWriter log)
{
    return await func.Handle(commandName, req, log);
}
