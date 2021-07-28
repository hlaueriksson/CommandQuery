#if NETCOREAPP3_1
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CommandQuery.AzureFunctions.Tests.V3
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this IActionResult result, string message, int? statusCode = null)
        {
            ShouldBeError(result as ContentResult, message, statusCode);
        }

        private static void ShouldBeError(this ContentResult result, string message, int? statusCode = null)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            if (statusCode.HasValue) result.StatusCode.Should().Be(statusCode);
            var value = JsonConvert.DeserializeObject<FakeError>(result.Content);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
#endif
