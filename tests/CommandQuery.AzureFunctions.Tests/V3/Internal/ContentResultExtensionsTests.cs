#if NETCOREAPP3_1
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V3.Internal
{
    public class ContentResultExtensionsTests
    {
        [Test]
        public async Task Ok()
        {
            var result = new { Foo = "Bar" }.Ok(null);
            result.StatusCode.Should().Be(200);
            result.Content.Should().Be("{\"Foo\":\"Bar\"}");
        }

        [Test]
        public async Task BadRequest()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var result = exception.BadRequest(null);
            result.StatusCode.Should().Be(400);
            result.Content.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
        }

        [Test]
        public async Task InternalServerError()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var result = exception.InternalServerError(null);
            result.StatusCode.Should().Be(500);
            result.Content.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
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
