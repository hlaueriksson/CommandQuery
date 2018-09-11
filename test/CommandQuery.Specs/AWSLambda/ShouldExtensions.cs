#if NETCOREAPP2_0
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.AWSLambda;
using Machine.Specifications;
using Newtonsoft.Json;

namespace CommandQuery.Specs.AWSLambda
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this APIGatewayProxyResponse result, string message)
        {
            result.ShouldNotBeNull();
            result.StatusCode.ShouldNotEqual(200);
            var value = JsonConvert.DeserializeObject<Error>(result.Body);
            value.ShouldNotBeNull();
            value.Message.ShouldEqual(message);
        }
    }
}
#endif