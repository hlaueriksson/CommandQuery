#if NETCOREAPP3_1
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.AzureFunctions.Tests.V3
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this IActionResult result, string message, int? statusCode = null)
        {
            ShouldBeError(result as JsonResult, message, statusCode);
        }

        private static void ShouldBeError(this JsonResult result, string message, int? statusCode = null)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            if (statusCode.HasValue) result.StatusCode.Should().Be(statusCode);
            var value = result.Value as IError;
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
#endif
