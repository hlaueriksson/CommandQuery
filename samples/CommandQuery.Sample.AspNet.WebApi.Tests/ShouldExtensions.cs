using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;

namespace CommandQuery.Sample.AspNet.WebApi.Tests
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this IHttpActionResult result, string message)
        {
            await (await result.ExecuteAsync(CancellationToken.None)).ShouldBeErrorAsync(message);
        }

        public static async Task ShouldBeErrorAsync(this HttpResponseMessage result, string message)
        {
            result.Should().NotBeNull();
            result.IsSuccessStatusCode.Should().BeFalse();
            var value = await result.Content.ReadAsStringAsync();
            value.Should().NotBeNull();
            value.Should().Contain(message);
        }
    }
}