using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AspNetCore.Tests
{
    public class QueryControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            FakeQueryProcessor = new Mock<IQueryProcessor>();
            Subject = new QueryController<FakeQuery, FakeResult>(FakeQueryProcessor.Object, null);
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandlePost(new FakeQuery()) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandlePost(new FakeQuery());

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                var result = await Subject.HandlePost(new FakeQuery());

                result.ShouldBeError("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleGet(new FakeQuery()) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandleGet(new FakeQuery());

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                var result = await Subject.HandleGet(new FakeQuery());

                result.ShouldBeError("fail", 500);
            }
        }

        Mock<IQueryProcessor> FakeQueryProcessor;
        private QueryController<FakeQuery, FakeResult> Subject;
    }
}