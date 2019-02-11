using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.Tests
{
    public class QueryControllerTests
    {
        public class when_using_the_real_controller_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            }

            [Test]
            public async Task should_work()
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var result = await Client.PostAsync("/api/query/BarQuery", content);
                var value = await result.Content.ReadAsAsync<Bar>();

                result.EnsureSuccessStatusCode();
                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var result = await Client.PostAsync("/api/query/FailQuery", content);

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            TestServer Server;
            HttpClient Client;
        }

        public class when_using_the_real_controller_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            }

            [Test]
            public async Task should_work()
            {
                var result = await Client.GetAsync("/api/query/BarQuery?Id=1");
                var value = await result.Content.ReadAsAsync<Bar>();

                result.EnsureSuccessStatusCode();
                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Client.GetAsync("/api/query/FailQuery?Id=1");

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            TestServer Server;
            HttpClient Client;
        }
    }
}