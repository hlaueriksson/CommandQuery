#if NET5_0
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V5.Internal
{
    public class HttpRequestDataExtensionsTests
    {
        private HttpRequestData Req;

        [SetUp]
        public void SetUp()
        {
            var context = new Mock<FunctionContext>();
            Req = new FakeHttpRequestData(context.Object);
        }

        [Test]
        public async Task OkAsync()
        {
            var response = await Req.OkAsync(new { Foo = "Bar" }, null, CancellationToken.None);
            response.StatusCode.Should().Be(200);
            response.Body.Position = 0;
            var result = await new StreamReader(response.Body).ReadToEndAsync();
            result.Should().Be("{\"Foo\":\"Bar\"}");
        }

        [Test]
        public async Task BadRequestAsync()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var response = await Req.BadRequestAsync(exception, null, CancellationToken.None);
            response.StatusCode.Should().Be(400);
            response.Body.Position = 0;
            var result = await new StreamReader(response.Body).ReadToEndAsync();
            result.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
        }

        [Test]
        public async Task InternalServerErrorAsync()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var response = await Req.InternalServerErrorAsync(exception, null, CancellationToken.None);
            response.StatusCode.Should().Be(500);
            response.Body.Position = 0;
            var result = await new StreamReader(response.Body).ReadToEndAsync();
            result.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
        }
    }

    public class CustomCommandException : CommandException
    {
        public string Foo { get; set; }

        public CustomCommandException(string message) : base(message)
        {
        }
    }
}
#endif
