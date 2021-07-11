using CommandQuery.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.AWSLambda.Tests.Internal
{
    public class ExceptionExtensionsTests
    {
        [Test]
        public void ToBadRequest()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var result = exception.ToBadRequest();
            result.StatusCode.Should().Be(400);
            result.Body.Should().Be("{\"Message\":\"fail\",\"Details\":{\"Foo\":\"Bar\"}}");
        }

        [Test]
        public void ToInternalServerError()
        {
            var exception = new CustomCommandException("fail") { Foo = "Bar" };
            var result = exception.ToInternalServerError();
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
