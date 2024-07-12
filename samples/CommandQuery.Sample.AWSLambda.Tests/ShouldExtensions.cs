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
            var value = result.Body<Error>()!;
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }

        public static T? Body<T>(this APIGatewayProxyResponse result)
        {
            return JsonSerializer.Deserialize<T>(result.Body);
        }
    }
}
