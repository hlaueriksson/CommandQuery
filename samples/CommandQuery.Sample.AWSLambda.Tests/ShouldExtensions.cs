using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.Sample.Contracts;
using FluentAssertions;

namespace CommandQuery.Sample.AWSLambda.Tests
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this APIGatewayProxyResponse result, string message)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            var value = JsonSerializer.Deserialize<Error>(result.Body)!;
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
