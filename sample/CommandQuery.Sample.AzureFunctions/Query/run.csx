#r "CommandQuery.dll"
#r "CommandQuery.AzureFunctions.dll"
#r "CommandQuery.Sample.dll"

using System.Net;
using System.Reflection;
using CommandQuery.AzureFunctions;

static QueryFunction func = new QueryFunction(Assembly.Load("CommandQuery.Sample").GetQueryProcessor());

public static async Task<HttpResponseMessage> Run(string queryName, HttpRequestMessage req, TraceWriter log)
{
    return await func.Handle(queryName, req, log);
}
