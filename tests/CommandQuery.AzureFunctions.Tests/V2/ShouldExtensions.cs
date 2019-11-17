#if NETCOREAPP2_2
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.AzureFunctions.Tests.V2
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this ObjectResult result, string message)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            var value = result.Value as Error;
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
#endif