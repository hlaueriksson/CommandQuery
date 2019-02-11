using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;

namespace CommandQuery.Sample.AspNetCore.Tests
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponseMessage result, string message)
        {
            result.Should().NotBeNull();
            result.IsSuccessStatusCode.Should().BeFalse();
            var value = await result.Content.ReadAsAsync<Error>();
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}