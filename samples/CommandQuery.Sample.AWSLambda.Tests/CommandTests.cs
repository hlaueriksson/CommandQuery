using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using CommandQuery.AWSLambda;
using CommandQuery.Sample.Contracts.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.Sample.AWSLambda.Tests
{
    public class CommandTests
    {
        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();
            new Startup().ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Subject = new Command(serviceProvider.GetRequiredService<ICommandFunction>());
            Context = new TestLambdaContext();
        }

        [Test]
        public async Task should_handle_command()
        {
            var response = await Subject.Post(GetRequest(new FooCommand { Value = "Foo" }), Context, "FooCommand");
            response.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task should_handle_errors()
        {
            var response = await Subject.Post(GetRequest(new FooCommand()), Context, "FooCommand");
            response.ShouldBeError("Value cannot be null or empty");
        }

        static APIGatewayProxyRequest GetRequest(object body) => new() { Body = JsonSerializer.Serialize(body) };

        Command Subject = null!;
        TestLambdaContext Context = null!;
    }
}
