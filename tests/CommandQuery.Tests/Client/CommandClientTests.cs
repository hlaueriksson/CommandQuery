using System.Text;
using System.Text.Json;
using CommandQuery.Client;
using CommandQuery.Sample.Contracts.Commands;
using FluentAssertions;
using Moq.Protected;

namespace CommandQuery.Tests.Client
{
    public class CommandClientTests
    {
        [SetUp]
        public void SetUp()
        {
            MockHandler = new Mock<HttpMessageHandler>();
            var client = new HttpClient(MockHandler.Object) { BaseAddress = new Uri("https://localhost") };
            Subject = new CommandClient(client);
        }

        [Test]
        public void Ctor()
        {
            new CommandClient("https://example.com", 20).Should().NotBeNull();
            new CommandClient("https://example.com", x => x.Timeout = TimeSpan.FromSeconds(20)).Should().NotBeNull();

            Action act = () => new CommandClient("https://example.com", null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public async Task PostAsync()
        {
            var command = new FooCommand { Value = "sv-SE" };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };

            MockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post && r.RequestUri.ToString().Contains(command.GetType().Name)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            await Subject.PostAsync(command);
        }

        [Test]
        public async Task PostAsync_with_result()
        {
            var expectation = new Baz { Success = true };
            var command = new BazCommand { Value = "sv-SE" };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectation), Encoding.UTF8, "application/json")
            };

            MockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post && r.RequestUri.ToString().Contains(command.GetType().Name)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            var result = await Subject.PostAsync(command);
            result.Should().BeEquivalentTo(expectation);
        }

        CommandClient Subject;
        Mock<HttpMessageHandler> MockHandler;
    }
}
