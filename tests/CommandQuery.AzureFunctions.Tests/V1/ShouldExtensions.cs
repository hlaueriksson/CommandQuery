#if NET472
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;

namespace CommandQuery.AzureFunctions.Tests.V1
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponseMessage result, string message, int? statusCode = null)
        {
            result.Should().NotBeNull();
            result.IsSuccessStatusCode.Should().BeFalse();
            if (statusCode.HasValue) result.StatusCode.Should().Be(statusCode);
            var value = await result.Content.ReadAsAsync<Error>();
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
#endif