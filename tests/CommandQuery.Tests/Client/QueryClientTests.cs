using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandQuery.Client;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CommandQuery.Tests.Client
{
    public class QueryClientTests
    {
        [SetUp]
        public void SetUp()
        {
            MockHandler = new Mock<HttpMessageHandler>();
            var client = new HttpClient(MockHandler.Object) { BaseAddress = new Uri("https://localhost") };
            Subject = new QueryClient(client);
        }

        [Test]
        public void Ctor()
        {
            new QueryClient("https://example.com", 20).Should().NotBeNull();
            new QueryClient("https://example.com", x => x.Timeout = TimeSpan.FromSeconds(20)).Should().NotBeNull();

            Action act = () => new QueryClient("https://example.com", null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public async Task PostAsync()
        {
            var expectation = new Bar { Id = 1, Value = "Value" };
            var query = new BarQuery { Id = 1 };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(expectation), Encoding.UTF8, "application/json")
            };

            MockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post && r.RequestUri.ToString().Contains(query.GetType().Name)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var result = await Subject.PostAsync(query);
            result.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public async Task GetAsync()
        {
            var expectation = new Bar { Id = 1, Value = "Value" };
            var query = new BarQuery { Id = 1 };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(expectation), Encoding.UTF8, "application/json")
            };

            MockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get && r.RequestUri.ToString().Contains(query.GetType().Name)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var result = await Subject.GetAsync(query);
            result.Should().BeEquivalentTo(expectation);
        }

        QueryClient Subject;
        Mock<HttpMessageHandler> MockHandler;
    }
}
