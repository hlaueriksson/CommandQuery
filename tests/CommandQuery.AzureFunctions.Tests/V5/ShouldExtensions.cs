#if NET6_0
using System.Text.Json;
using System.Threading.Tasks;
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.AzureFunctions.Tests.V5
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponseData result, string message, int? statusCode = null)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            if (statusCode.HasValue) result.StatusCode.Should().Be(statusCode);
            result.Body.Position = 0;
            var value = await JsonSerializer.DeserializeAsync<FakeError>(result.Body);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
#endif
