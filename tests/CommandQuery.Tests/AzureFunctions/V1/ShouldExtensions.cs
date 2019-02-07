#if NET471
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;

namespace CommandQuery.Tests.AzureFunctions.V1
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponseMessage result, string message)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(HttpStatusCode.OK);
            var value = await result.Content.ReadAsAsync<Error>();
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
#endif