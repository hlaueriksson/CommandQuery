using System.Net.Http.Json;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Google.Cloud.Functions.Testing;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace CommandQuery.Sample.GoogleCloudFunctions.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                Server = new FunctionTestServer<Query>();
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
                var response = await Client.PostAsJsonAsync("/api/query/BarQuery", new BarQuery { Id = 1 });
                var value = await response.Content.ReadFromJsonAsync<Bar>();
                value!.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var response = await Client.PostAsJsonAsync("/api/query/FailQuery", new BarQuery { Id = 1 });
                await response.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            FunctionTestServer<Query> Server = null!;
            HttpClient Client = null!;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                Server = new FunctionTestServer<Query>();
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
                var response = await Client.GetAsync("/api/query/BarQuery?Id=1");
                var value = await response.Content.ReadFromJsonAsync<Bar>();
                value!.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var response = await Client.GetAsync("/api/query/FailQuery?Id=1");
                await response.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            FunctionTestServer<Query> Server = null!;
            HttpClient Client = null!;
        }
    }
}
