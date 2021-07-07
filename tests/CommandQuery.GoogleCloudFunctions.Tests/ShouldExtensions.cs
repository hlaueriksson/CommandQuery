using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.GoogleCloudFunctions.Tests
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponse response, string message, int? statusCode = null)
        {
            response.Should().NotBeNull();
            response.StatusCode.Should().NotBe(200);
            if (statusCode.HasValue) response.StatusCode.Should().Be(statusCode);
            response.Body.Seek(0, SeekOrigin.Begin);
            var value = await JsonSerializer.DeserializeAsync<FakeError>(response.Body);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
