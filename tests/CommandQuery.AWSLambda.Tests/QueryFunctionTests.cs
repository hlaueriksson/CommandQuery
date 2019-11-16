using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
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
            Context = new Mock<ILambdaContext>();
            Context.SetupGet(x => x.Logger).Returns(new Mock<ILambdaLogger>().Object);
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Request = new APIGatewayProxyRequest { Body = "{}" };

            async Task should_invoke_the_query_processor()
            {
                await Subject.Handle(QueryName, Request, Context.Object);

                The<Mock<IQueryProcessor>>().Verify(x => x.ProcessAsync<object>(QueryName, Request.Body));
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, Request.Body)).Throws(new Exception("fail"));

                var result = await Subject.Handle(QueryName, Request, Context.Object);

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Request = new APIGatewayProxyRequest { HttpMethod = "GET" };

            async Task should_invoke_the_query_processor()
            {
                await Subject.Handle(QueryName, Request, Context.Object);

                The<Mock<IQueryProcessor>>().Verify(x => x.ProcessAsync<object>(QueryName, Request.QueryStringParameters));
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, Request.QueryStringParameters)).Throws(new Exception("fail"));

                var result = await Subject.Handle(QueryName, Request, Context.Object);

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        APIGatewayProxyRequest Request;
        Mock<ILambdaContext> Context;
        string QueryName = "FakeQuery";
    }
}