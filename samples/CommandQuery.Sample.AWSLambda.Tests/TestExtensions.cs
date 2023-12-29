using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace CommandQuery.Sample.AWSLambda.Tests
{
    public static class TestExtensions
    {
        public static T? As<T>(this APIGatewayProxyResponse result)
        {
            return JsonSerializer.Deserialize<T>(result.Body);
        }
    }
}
