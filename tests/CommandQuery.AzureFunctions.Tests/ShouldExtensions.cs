using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.AzureFunctions.Tests
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this IActionResult result, string message, int? statusCode = null)
        {
            ShouldBeError(result as ObjectResult, message, statusCode);
        }

        public static void ShouldBeError(this ObjectResult result, string message, int? statusCode = null)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            if (statusCode.HasValue) result.StatusCode.Should().Be(statusCode);
            var value = result.Value as Error;
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
