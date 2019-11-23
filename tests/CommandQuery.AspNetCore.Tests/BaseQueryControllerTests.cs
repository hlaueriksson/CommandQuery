using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.AspNetCore.Tests
{
    public class BaseQueryControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            FakeQueryProcessor = new Mock<IQueryProcessor>();
            Subject = new FakeQueryController(FakeQueryProcessor.Object)
            {
                ControllerContext = Fake.ControllerContext()
            };
            Json = JObject.Parse("{}");
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            QueryName = "FakeQuery";
            FakeQueryProcessor.Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandlePost(QueryName, Json) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandlePost(QueryName, Json);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                var result = await Subject.HandlePost(QueryName, Json);

                result.ShouldBeError("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            QueryName = "FakeQuery";
            FakeQueryProcessor.Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleGet(QueryName) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandleGet(QueryName);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                var result = await Subject.HandleGet(QueryName);

                result.ShouldBeError("fail", 500);
            }
        }

        Mock<IQueryProcessor> FakeQueryProcessor;
        BaseQueryController Subject;
        string QueryName;
        JObject Json;
    }
}