using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.Vs2.Tests
{
    public class CommandTests
    {
        public class when_using_the_real_function
        {
            [Test]
            public async Task should_work()
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new Mock<ILogger>();

                var result = await Command.Run(req, log.Object, "FooCommand") as EmptyResult;

                result.Should().NotBeNull();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new Mock<ILogger>();

                var result = await Command.Run(req, log.Object, "FailCommand") as BadRequestObjectResult;

                result.ShouldBeError("The command type 'FailCommand' could not be found");
            }

            DefaultHttpRequest GetHttpRequest(string content)
            {
                var httpContext = new DefaultHttpContext();
                httpContext.Features.Get<IHttpRequestFeature>().Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

                return new DefaultHttpRequest(httpContext);
            }
        }
    }
}