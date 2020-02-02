using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.V3.Tests
{
    public class CommandControllerTests
    {
        public class when_using_the_real_controller
        {
            [SetUp]
            public void SetUp()
            {
                var factory = new WebApplicationFactory<Startup>();
                Client = factory.CreateClient();
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

            HttpClient Client;
        }
    }
}