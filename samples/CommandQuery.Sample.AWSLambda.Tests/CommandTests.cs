using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Sample.AWSLambda.Tests
{
    public class CommandTests
    {
        public class when_using_the_real_function
        {
            [Test]
            public async Task should_work()
            {
                var request = GetRequest("{ 'Value': 'Foo' }");
                var context = new TestLambdaContext();

                var result = await new Command().Handle(request.CommandName("FooCommand"), context);

                result.Should().NotBeNull();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var request = GetRequest("{ 'Value': 'Foo' }");
                var context = new TestLambdaContext();

                var result = await new Command().Handle(request.CommandName("FailCommand"), context);

                result.ShouldBeError("The command type 'FailCommand' could not be found");
            }

            APIGatewayProxyRequest GetRequest(string content) => new APIGatewayProxyRequest { Body = content };
        }
    }
}
