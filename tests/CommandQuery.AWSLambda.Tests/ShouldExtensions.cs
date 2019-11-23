using Amazon.Lambda.APIGatewayEvents;
using FluentAssertions;
using Newtonsoft.Json;

namespace CommandQuery.AWSLambda.Tests
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this APIGatewayProxyResponse result, string message, int? statusCode = null)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            if (statusCode.HasValue) result.StatusCode.Should().Be(statusCode);
            var value = JsonConvert.DeserializeObject<Error>(result.Body);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}