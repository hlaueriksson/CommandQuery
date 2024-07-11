using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.AWSLambda.Tests.Internal
{
    public class APIGatewayProxyRequestExtensionsTests
    {
        private readonly APIGatewayProxyRequest _request;

        [Test]
        public void Ok()
        {
            var result = _request.Ok(new { Foo = "Bar" });
            result.StatusCode.Should().Be(200);
            result.Body.Should().Be("{\"Foo\":\"Bar\"}");
        }

        [Test]
        public void BadRequest()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var result = _request.BadRequest(exception);
            result.StatusCode.Should().Be(400);
            result.Body.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
        }

        [Test]
        public void InternalServerError()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var result = _request.InternalServerError(exception);
            result.StatusCode.Should().Be(500);
            result.Body.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
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
