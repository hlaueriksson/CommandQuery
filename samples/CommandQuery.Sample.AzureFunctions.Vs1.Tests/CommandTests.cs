using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.Vs1.Tests
{
    public class CommandTests
    {
        public class when_using_the_real_function
        {
            [Test]
            public async Task should_work()
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new FakeTraceWriter();

                var result = await Command.Run(req, log, "FooCommand");

                result.Should().NotBeNull();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new FakeTraceWriter();

                var result = await Command.Run(req, log, "FailCommand");

                await result.ShouldBeErrorAsync("The command type 'FailCommand' could not be found");
            }

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