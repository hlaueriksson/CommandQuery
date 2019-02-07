#if NETCOREAPP2_0
using Amazon.Lambda.APIGatewayEvents;
using FluentAssertions;
using Newtonsoft.Json;

namespace CommandQuery.Tests.AWSLambda
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this APIGatewayProxyResponse result, string message)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            var value = JsonConvert.DeserializeObject<Error>(result.Body);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
#endif