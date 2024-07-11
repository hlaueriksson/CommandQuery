using CommandQuery.Client;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Tests.Client.Integration
{
    [Explicit("Integration tests")]
    public class QueryClientIntegrationTests
    {
        [SetUp]
        public void SetUp()
        {
            Subject = new QueryClient("https://commandquery-sample-azurefunctions-vs3.azurewebsites.net/api/query/");
        }

        [Test]
        public async Task when_PostAsync()
        {
            var result = await Subject.PostAsync(new BarQuery { Id = 1 });
            result.Should().NotBeNull();

            Func<Task> act = () => Subject.PostAsync(new FailQuery());
            await act.Should().ThrowAsync<CommandQueryException>();
        }

        [Test]
        public async Task when_GetAsync()
        {
            var result = await Subject.GetAsync(new QuxQuery { Ids = [Guid.NewGuid(), Guid.NewGuid()] });
            result.Should().NotBeNull();

            Func<Task> act = () => Subject.GetAsync(new FailQuery());
            await act.Should().ThrowAsync<CommandQueryException>();
        }

        [Test]
        public async Task when_configuring_the_client()
        {
            var client = new QueryClient("http://example.com", x => x.BaseAddress = new Uri("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/query/"));
            await client.PostAsync(new BarQuery { Id = 1 });
        }

        QueryClient Subject;
    }

    public class FailQuery : IQuery<object> { }
}
