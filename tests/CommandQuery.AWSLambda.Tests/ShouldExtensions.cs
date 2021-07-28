using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.Tests;
using FluentAssertions;

namespace CommandQuery.AWSLambda.Tests
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this APIGatewayProxyResponse result, string message, int? statusCode = null)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            if (statusCode.HasValue) result.StatusCode.Should().Be(statusCode);
            var value = JsonSerializer.Deserialize<FakeError>(result.Body);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
