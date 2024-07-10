using System.Net;
using System.Text.Json;
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.AzureFunctions.Tests
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponseData result, string message, HttpStatusCode? statusCode = null)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(HttpStatusCode.OK);
            if (statusCode.HasValue) result.StatusCode.Should().Be(statusCode);
            result.Body.Position = 0;
            var value = await JsonSerializer.DeserializeAsync<FakeError>(result.Body);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
