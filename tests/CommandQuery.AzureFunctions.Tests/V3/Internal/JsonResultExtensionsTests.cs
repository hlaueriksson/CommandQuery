#if NETCOREAPP3_1
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V3.Internal
{
    public class JsonResultExtensionsTests
    {
        [Test]
        public async Task Ok()
        {
            var result = new { Foo = "Bar" }.Ok(null);
            result.StatusCode.Should().Be(200);

            result = new { Foo = "Bar" }.Ok(new JsonSerializerSettings());
            result.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task BadRequest()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var result = exception.BadRequest(null);
            result.StatusCode.Should().Be(400);

            result = exception.BadRequest(new JsonSerializerSettings());
            result.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task InternalServerError()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var response = exception.InternalServerError(null);
            response.StatusCode.Should().Be(500);

            response = exception.InternalServerError(new JsonSerializerSettings());
            response.StatusCode.Should().Be(500);
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
