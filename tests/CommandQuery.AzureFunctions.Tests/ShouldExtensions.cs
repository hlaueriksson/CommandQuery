using System.Net;
using System.Text.Json;
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.AzureFunctions.Tests
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponseData result, string message, HttpStatusCode? statusCode = null)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(HttpStatusCode.OK);
            if (statusCode.HasValue) result.StatusCode.Should().Be(statusCode);
            result.Body.Position = 0;
            var value = await JsonSerializer.DeserializeAsync<FakeError>(result.Body);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }

        public static void ShouldBeError(this IActionResult result, string message, int? statusCode = null)
        {
            result.Should().NotBeNull();

            var resultWithStatusCode = result as IStatusCodeActionResult;
            resultWithStatusCode.StatusCode.Should().NotBe(200);
            if (statusCode.HasValue) resultWithStatusCode.StatusCode.Should().Be(statusCode);

            var resultWithValue = result as ObjectResult;
            var value = resultWithValue.Value as IError;
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
