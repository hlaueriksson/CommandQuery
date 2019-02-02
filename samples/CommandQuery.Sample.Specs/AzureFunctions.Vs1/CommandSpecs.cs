#if NET461
using System.Net.Http;
using System.Web.Http;
using CommandQuery.Sample.AzureFunctions.Vs1;
using Machine.Specifications;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs1
{
    public class CommandSpecs
    {
        [Subject(typeof(Command))]
        public class when_using_the_real_function
        {
            It should_work = () =>
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new FakeTraceWriter();

                var result = Command.Run(req, log, "FooCommand").Result;

                result.ShouldNotBeNull();
            };

            It should_handle_errors = () =>
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new FakeTraceWriter();

                var result = Command.Run(req, log, "FailCommand").Result;

                result.ShouldBeError("The command type 'FailCommand' could not be found");
            };

            static HttpRequestMessage GetHttpRequest(string content)
            {
                var config = new HttpConfiguration();
                var request = new HttpRequestMessage();
                request.SetConfiguration(config);
                request.Content = new StringContent(content);

                return request;
            }
        }
    }
}
#endif