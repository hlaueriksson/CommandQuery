using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AWSLambda.Tests
{
    public class QueryFunctionTests : LoFuTest<QueryFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            Use<Mock<IQueryProcessor>>();
            Logger = new Mock<ILambdaLogger>().Object;
            QueryName = "FakeQuery";
            The<Mock<IQueryProcessor>>().Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Request = new APIGatewayProxyRequest { Body = "{}" };

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).ReturnsAsync(expected);

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Should().NotBeEmpty();
            }

            async Task should_throw_when_request_is_null()
            {
                Subject.Awaiting(x => x.HandleAsync(QueryName, null, Logger))
                    .Should().Throw<ArgumentNullException>();
            }

            async Task should_handle_QueryProcessorException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_QueryException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryException("invalid"));

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

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
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).ReturnsAsync(expected);

                var result = await Subject.HandleAsync(QueryName, Request, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Should().NotBeEmpty();
            }
        }

        APIGatewayProxyRequest Request;
        ILambdaLogger Logger;
        string QueryName;
    }
}
