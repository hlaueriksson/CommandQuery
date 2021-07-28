using CommandQuery.Sample.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CommandQuery.Sample.AzureFunctions.V3.Tests
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this ContentResult result, string message)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
            var value = JsonConvert.DeserializeObject<Error>(result.Content);
            value.Should().NotBeNull();
            value.Message.Should().Be(message);
        }
    }
}
