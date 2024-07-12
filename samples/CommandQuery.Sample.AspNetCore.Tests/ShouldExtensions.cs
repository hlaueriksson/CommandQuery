using System.Net;
using System.Net.Http.Json;
using CommandQuery.Sample.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.Sample.AspNetCore.Tests
{
    public static class ShouldExtensions
    {
        public static async Task ShouldBeErrorAsync(this HttpResponseMessage result, string message)
        {
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(HttpStatusCode.OK);
            var value = await result.Content.ReadFromJsonAsync<Error>();
            value.Should().NotBeNull();
            value!.Message.Should().Be(message);
        }
    }
}
