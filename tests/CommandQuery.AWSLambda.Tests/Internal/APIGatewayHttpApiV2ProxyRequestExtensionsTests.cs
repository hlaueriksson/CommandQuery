using Amazon.Lambda.APIGatewayEvents;
using FluentAssertions;

namespace CommandQuery.AWSLambda.Tests.Internal
{
    public class APIGatewayHttpApiV2ProxyRequestExtensionsTests
    {
        private readonly APIGatewayHttpApiV2ProxyRequest _request = null;

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
}
