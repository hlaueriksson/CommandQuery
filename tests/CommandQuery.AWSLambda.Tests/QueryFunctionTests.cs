using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;

namespace CommandQuery.AWSLambda.Tests
{
    public class QueryFunctionTests_APIGatewayProxyRequest : LoFuTest<QueryFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            QueryName = "FakeQuery";
            Use<Mock<IQueryProcessor>>().Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));
            Use<JsonSerializerOptions>(null);
            Logger = One<ILambdaLogger>();
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Request = new APIGatewayProxyRequest { Body = "{}" };

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Should().NotBeEmpty();
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(QueryName, (APIGatewayProxyRequest)null, Logger);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_QueryProcessorException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_QueryException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryException("invalid"));

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.ShouldBeError("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Request = new APIGatewayProxyRequest { HttpMethod = "GET", MultiValueQueryStringParameters = new Dictionary<string, IList<string>> { { "foo", new List<string> { "bar" } } } };

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Should().NotBeEmpty();
            }
        }

        APIGatewayProxyRequest Request;
        ILambdaLogger Logger;
        string QueryName;
    }

    public class QueryFunctionTests_APIGatewayHttpApiV2ProxyRequest : LoFuTest<QueryFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            QueryName = "FakeQuery";
            Use<Mock<IQueryProcessor>>().Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));
            Use<JsonSerializerOptions>(null);
            Logger = One<ILambdaLogger>();
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Request = new APIGatewayHttpApiV2ProxyRequest
            {
                RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext { Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription { Method = "POST", Path = "" } },
                Body = "{}",
            };

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Should().NotBeEmpty();
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(QueryName, (APIGatewayHttpApiV2ProxyRequest)null, Logger);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_QueryProcessorException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_QueryException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryException("invalid"));

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.ShouldBeError("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Request = new APIGatewayHttpApiV2ProxyRequest
            {
                RequestContext = new APIGatewayHttpApiV2ProxyRequest.ProxyRequestContext { Http = new APIGatewayHttpApiV2ProxyRequest.HttpDescription { Method = "GET" } },
                QueryStringParameters = new Dictionary<string, string> { { "foo", "bar" } },
            };

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Should().NotBeEmpty();
            }
        }

        APIGatewayHttpApiV2ProxyRequest Request;
        ILambdaLogger Logger;
        string QueryName;
    }
}
