using System.Net;
using System.Net.Http.Json;
using CommandQuery.Sample.Contracts.Commands;
using FluentAssertions;
using Google.Cloud.Functions.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.GoogleCloudFunctions.Tests
{
    public class CommandTests
    {
        public class when_using_the_real_function
        {
            [SetUp]
            public void SetUp()
            {
                Server = new FunctionTestServer<Command>();
                Client = Server.CreateClient();
            }

            [TearDown]
            public void TearDown()
            {
                Client.Dispose();
                Server.Dispose();
            }

            [Test]
            public async Task should_work()
            {
                var response = await Client.PostAsJsonAsync("/api/command/FooCommand", new FooCommand { Value = "Foo" });
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            [Test]
            public async Task should_handle_errors()
            {
                var response = await Client.PostAsJsonAsync("/api/command/FailCommand", new FooCommand { Value = "Foo" });
                await response.ShouldBeErrorAsync("The command type 'FailCommand' could not be found");
            }

            FunctionTestServer<Command> Server = null!;
            HttpClient Client = null!;
        }
    }
}
