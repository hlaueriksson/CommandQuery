using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CommandQuery.Sample.Contracts;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.Sample.AzureFunctions.V6.Tests
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponseData result, string message)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(HttpStatusCode.OK);
            result.Body.Position = 0;
            var value = await JsonSerializer.DeserializeAsync<Error>(result.Body);
            value.Should().NotBeNull();
            value!.Message.Should().Be(message);
        }
    }
}
