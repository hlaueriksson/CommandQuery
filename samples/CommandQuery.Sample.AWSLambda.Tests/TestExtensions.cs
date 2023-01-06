using System.Collections.Generic;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace CommandQuery.Sample.AWSLambda.Tests
{
    public static class TestExtensions
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

        public static T? As<T>(this APIGatewayProxyResponse result)
        {
            return JsonSerializer.Deserialize<T>(result.Body);
        }
    }
}
