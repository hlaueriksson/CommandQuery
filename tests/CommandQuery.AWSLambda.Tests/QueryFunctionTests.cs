using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.Exceptions;
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
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            async Task should_invoke_the_query_processor()
            {
                var queryName = "FakeQuery";
                var content = "{}";

                await Subject.Handle(queryName, content);

                The<Mock<IQueryProcessor>>().Verify(x => x.ProcessAsync<object>(queryName, content));
            }

            async Task base_method_should_not_handle_Exception()
            {
                var queryName = "FakeQuery";
                var content = "{}";

                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, content)).Throws(new Exception("fail"));

                Subject.Awaiting(async x => await x.Handle(queryName, content)).Should()
                    .Throw<Exception>()
                    .WithMessage("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            async Task should_invoke_the_query_processor()
            {
                var queryName = "FakeQuery";
                var query = new Dictionary<string, string>();

                await Subject.Handle(queryName, query);

                The<Mock<IQueryProcessor>>().Verify(x => x.ProcessAsync<object>(queryName, query));
            }

            async Task base_method_should_not_handle_Exception()
            {
                var queryName = "FakeQuery";
                var query = new Dictionary<string, string>();

                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, query)).Throws(new Exception("fail"));

                Subject.Awaiting(async x => await x.Handle(queryName, query)).Should()
                    .Throw<Exception>()
                    .WithMessage("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_lambda_Request()
        {
            Request = new APIGatewayProxyRequest();
            Context = new Mock<ILambdaContext>();
            Context.SetupGet(x => x.Logger).Returns(new Mock<ILambdaLogger>().Object);

            async Task should_handle_QueryValidationException()
            {
                var queryName = "FakeQuery";
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, It.IsAny<string>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(queryName, Request, Context.Object);

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                var queryName = "FakeQuery";
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(queryName, Request, Context.Object);

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        APIGatewayProxyRequest Request;
        Mock<ILambdaContext> Context;
    }
}