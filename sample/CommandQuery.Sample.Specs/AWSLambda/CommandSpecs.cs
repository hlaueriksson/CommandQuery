#if NETCOREAPP2_0
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.Sample.AWSLambda;
using Machine.Specifications;

namespace CommandQuery.Sample.Specs.AWSLambda
{
    public class CommandSpecs
    {
        [Subject(typeof(Command))]
        public class when_using_the_real_function
        {
            It should_work = () =>
            {
                var request = GetRequest("{ 'Value': 'Foo' }");
                var context = new FakeLambdaContext();

                var result = new Command().Handle(request.CommandName("FooCommand"), context).Result;

                result.ShouldNotBeNull();
            };

            It should_handle_errors = () =>
            {
                var request = GetRequest("{ 'Value': 'Foo' }");
                var context = new FakeLambdaContext();

                var result = new Command().Handle(request.CommandName("FailCommand"), context).Result;

                result.ShouldBeError("The command type 'FailCommand' could not be found");
            };

            static APIGatewayProxyRequest GetRequest(string content) => new APIGatewayProxyRequest { Body = content };
        }
    }
}
#endif