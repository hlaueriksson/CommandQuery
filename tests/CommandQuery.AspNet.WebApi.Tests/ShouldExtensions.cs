using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;

namespace CommandQuery.AspNet.WebApi.Tests
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this IHttpActionResult result, string message)
        {
            var response = await result.ExecuteAsync(CancellationToken.None);
            response.Should().NotBeNull();
            response.StatusCode.Should().NotBe(HttpStatusCode.OK);
            var value = await response.Content.ReadAsAsync<Error>();
            value.Should().NotBeNull();

            if (response.StatusCode != HttpStatusCode.InternalServerError) value.Message.Should().Be(message);
        }
    }
}