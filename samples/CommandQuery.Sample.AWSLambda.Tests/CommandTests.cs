using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using CommandQuery.AWSLambda;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.Sample.AWSLambda.Tests
{
    public class CommandTests
    {
        public class when_using_the_real_function
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Command(serviceProvider.GetRequiredService<ICommandFunction>());
            }

            [Test]
            public async Task should_work()
            {
                var request = GetRequest("{ 'Value': 'Foo' }");
                var context = new TestLambdaContext();

                var result = await Subject.Handle(request, context, "FooCommand");

                result.Should().NotBeNull();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var request = GetRequest("{ 'Value': 'Foo' }");
                var context = new TestLambdaContext();

                var result = await Subject.Handle(request, context, "FailCommand");

                result.ShouldBeError("The command type 'FailCommand' could not be found");
            }

            static APIGatewayProxyRequest GetRequest(string content) => new() { Body = content };

            Command Subject = null!;
        }
    }
}
