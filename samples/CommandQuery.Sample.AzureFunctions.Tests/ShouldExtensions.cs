using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace CommandQuery.Sample.AzureFunctions.Tests
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this IActionResult result, string message)
        {
            result.Should().NotBeNull();
            result.As<IStatusCodeActionResult>().StatusCode.Should().NotBe(200);
            var value = result.Value<IError>()!;
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }

        public static T As<T>(this IActionResult result)
        {
            return (T)result;
        }

        public static T? Value<T>(this IActionResult result) where T : class
        {
            return result.As<ObjectResult>().Value as T;
        }
    }
}
