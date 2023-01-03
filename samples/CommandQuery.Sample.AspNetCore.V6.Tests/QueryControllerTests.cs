using System.Net;
using System.Text;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNetCore.V6.Tests
{
    public class QueryControllerTests
    {
        public class when_using_the_real_controller_via_Post
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
                var content = new StringContent("{ \"Id\": 1 }", Encoding.UTF8, "application/json");

                var result = await Client.PostAsync("/api/query/BarQuery", content);
                var value = await result.Content.ReadAsAsync<Bar>();

                result.EnsureSuccessStatusCode();
                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var content = new StringContent("{ \"Id\": 1 }", Encoding.UTF8, "application/json");

                var result = await Client.PostAsync("/api/query/FailQuery", content);

                result.StatusCode.Should().Be(HttpStatusCode.NotFound);
                (await result.Content.ReadAsStringAsync()).Should().BeEmpty();
            }

            WebApplicationFactory<Program> Factory = null!;
            HttpClient Client = null!;
        }

        public class when_using_the_real_controller_via_Get
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

                result.StatusCode.Should().Be(HttpStatusCode.NotFound);
                (await result.Content.ReadAsStringAsync()).Should().BeEmpty();
            }

            WebApplicationFactory<Program> Factory = null!;
            HttpClient Client = null!;
        }
    }
}
