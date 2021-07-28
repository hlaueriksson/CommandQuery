using System.Text.Json;
using System.Threading.Tasks;
using CommandQuery.Sample.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.Sample.GoogleCloudFunctions.Tests
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponse result, string message)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            result.Body.Position = 0;
            var value = await JsonSerializer.DeserializeAsync<Error>(result.Body);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
