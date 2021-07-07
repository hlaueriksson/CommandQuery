using System;
using System.Threading.Tasks;
using CommandQuery.Client;
using CommandQuery.Sample.Contracts.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Tests.Client.Integration
{
    [Explicit("Integration tests")]
    public class CommandClientIntegrationTests
    {
        [SetUp]
        public void SetUp()
        {
            Subject = new CommandClient("https://commandquery-sample-azurefunctions-vs3.azurewebsites.net/api/command/");
        }

        [Test]
        public async Task when_PostAsync()
        {
            await Subject.PostAsync(new FooCommand { Value = "sv-SE" });

            Subject.Awaiting(x => x.PostAsync(new FooCommand()))
                .Should().Throw<CommandQueryException>()
                .And.Error.GetErrorCode().Should().Be(1337);

            Subject.Awaiting(x => x.PostAsync(new FailCommand()))
                .Should().Throw<CommandQueryException>();
        }

        [Test]
        public async Task when_PostAsync_with_result()
        {
            var result = await Subject.PostAsync(new BazCommand { Value = "sv-SE" });
            result.Should().NotBeNull();

            Subject.Awaiting(x => x.PostAsync(new FailResultCommand()))
                .Should().Throw<CommandQueryException>();
        }

        [Test]
        public async Task when_configuring_the_client()
        {
            var client = new CommandClient("http://example.com", x => x.BaseAddress = new Uri("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/command/"));
            await client.PostAsync(new FooCommand { Value = "sv-SE" });
        }

        CommandClient Subject;
    }

    public class FailCommand : ICommand { }
    public class FailResultCommand : ICommand<object> { }

    public static class FooCommandExceptionExtensions
    {
        public static long? GetErrorCode(this IError error)
        {
            return error?.Details?["ErrorCode"] as long?;
        }
    }
}