using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.V6.Tests
{
    public class CommandControllerTests
    {
        public class when_using_the_real_controller
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
            public async Task should_work()
            {
                var content = new StringContent("{ \"Value\": \"Foo\" }", Encoding.UTF8, "application/json");

                var result = await Client.PostAsync("/api/command/FooCommand", content);

                result.EnsureSuccessStatusCode();
                (await result.Content.ReadAsStringAsync()).Should().BeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var content = new StringContent("{ \"Value\": \"Foo\" }", Encoding.UTF8, "application/json");

                var result = await Client.PostAsync("/api/command/FailCommand", content);

                result.StatusCode.Should().Be(HttpStatusCode.NotFound);
                (await result.Content.ReadAsStringAsync()).Should().BeEmpty();
            }

            WebApplicationFactory<Program> Factory;
            HttpClient Client;
        }
    }
}
