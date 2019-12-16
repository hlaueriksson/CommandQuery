using System;
using System.Threading.Tasks;
using CommandQuery.Client;
using CommandQuery.Sample.Contracts.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Tests.Client
{
    [Explicit("Integration tests")]
    public class CommandClientTests
    {
        [SetUp]
        public void SetUp()
        {
            Subject = new CommandClient("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/command/");
        }

        [Test]
        public void when_Post()
        {
            Subject.Post(new FooCommand { Value = "sv-SE" });
        }

        [Test]
        public void when_Post_with_result()
        {
            var result = Subject.Post(new BazCommand { Value = "sv-SE" });
            result.Should().NotBeNull();
        }

        [Test]
        public async Task when_PostAsync()
        {
            await Subject.PostAsync(new FooCommand { Value = "sv-SE" });
        }

        [Test]
        public async Task when_PostAsync_with_result()
        {
            var result = await Subject.PostAsync(new BazCommand { Value = "sv-SE" });
            result.Should().NotBeNull();
        }

        [Test]
        public void when_configuring_the_client()
        {
            var client = new CommandClient("http://example.com", x => x.BaseAddress = new Uri("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/command/"));
            client.Post(new FooCommand { Value = "sv-SE" });
        }

        CommandClient Subject;
    }
}