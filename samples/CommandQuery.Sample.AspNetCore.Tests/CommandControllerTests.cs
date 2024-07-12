using System.Net;
using CommandQuery.Sample.Contracts.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.Tests
{
    public class CommandControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            Factory = new WebApplicationFactory<Program>();
            Client = Factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            Client.Dispose();
            Factory.Dispose();
        }

        [Test]
        public async Task should_handle_command()
        {
            var response = await Client.PostAsJsonAsync("/api/command/FooCommand", new FooCommand { Value = "Foo" });
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task should_handle_errors()
        {
            var response = await Client.PostAsJsonAsync("/api/command/FooCommand", new FooCommand { Value = "" });
            await response.ShouldBeErrorAsync("Value cannot be null or empty");
        }

        WebApplicationFactory<Program> Factory = null!;
        HttpClient Client = null!;
    }
}
