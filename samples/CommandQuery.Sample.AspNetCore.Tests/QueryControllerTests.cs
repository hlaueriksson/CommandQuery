using System.Net.Http.Json;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.Tests
{
    public class QueryControllerTests
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

        WebApplicationFactory<Program> Factory = null!;
        HttpClient Client = null!;
    }
}
