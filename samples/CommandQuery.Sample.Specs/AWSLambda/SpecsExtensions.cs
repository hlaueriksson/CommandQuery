#if NETCOREAPP2_0
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace CommandQuery.Sample.Specs.AWSLambda
{
    public static class SpecsExtensions
    {
        public static APIGatewayProxyRequest QueryName(this APIGatewayProxyRequest request, string queryName)
        {
            request.PathParameters = new Dictionary<string, string> { { "queryName", queryName } };

            return request;
        }

        public static APIGatewayProxyRequest CommandName(this APIGatewayProxyRequest request, string commandName)
        {
            request.PathParameters = new Dictionary<string, string> { { "commandName", commandName } };

            return request;
        }

        public static T As<T>(this APIGatewayProxyResponse result)
        {
            return JsonConvert.DeserializeObject<T>(result.Body);
        }
    }
}
#endif