using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace CommandQuery.GoogleCloudFunctions.Tests.Internal
{
    public class HttpExtensionsTests
    {
        private HttpRequest Request;
        private HttpResponse Response;

        [SetUp]
        public void SetUp()
        {
            var context = new DefaultHttpContext();
            Request = context.Request;
            Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            Response = context.Response;
            Response.Body = new MemoryStream();
        }

        [Test]
        public async Task ReadAsStringAsync()
        {
            var result = await Request.ReadAsStringAsync();
            result.Should().Be("{}");
        }

        [Test]
        public async Task OkAsync()
        {
            await Response.OkAsync(new { Foo = "Bar" }, null, CancellationToken.None);
            Response.StatusCode.Should().Be(200);
            Response.Body.Position = 0;
            var result = await new StreamReader(Response.Body).ReadToEndAsync();
            result.Should().Be("{\"Foo\":\"Bar\"}");
        }

        [Test]
        public async Task BadRequestAsync()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            await Response.BadRequestAsync(exception, null, CancellationToken.None);
            Response.StatusCode.Should().Be(400);
            Response.Body.Position = 0;
            var result = await new StreamReader(Response.Body).ReadToEndAsync();
            result.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
        }

        [Test]
        public async Task InternalServerErrorAsync()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            await Response.InternalServerErrorAsync(exception, null, CancellationToken.None);
            Response.StatusCode.Should().Be(500);
            Response.Body.Position = 0;
            var result = await new StreamReader(Response.Body).ReadToEndAsync();
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
