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
        public async Task should_handle_query_via_Post()
        {
            var response = await Client.PostAsJsonAsync("/api/query/BarQuery", new BarQuery { Id = 1 });
            var value = await response.Content.ReadFromJsonAsync<Bar>();
            value!.Id.Should().Be(1);
        }

        [Test]
        public async Task should_handle_query_via_Get()
        {
            var response = await Client.GetAsync("/api/query/BarQuery?Id=1");
            var value = await response.Content.ReadFromJsonAsync<Bar>();
            value!.Id.Should().Be(1);
        }

        FunctionTestServer<Query> Server = null!;
        HttpClient Client = null!;
    }
}
