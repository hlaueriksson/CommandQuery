using System;
using System.Collections.Generic;
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
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            QueryName = "FakeQuery";
            Json = JObject.Parse("{}");

            async Task should_invoke_the_query_processor()
            {
                await Subject.HandlePost(QueryName, Json);

                FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(QueryName, Json));
            }

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new object();
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, Json)).Returns(Task.FromResult(expected));

                var result = await Subject.HandlePost(QueryName, Json) as OkObjectResult;

                result.Value.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, Json)).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandlePost(QueryName, Json) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, Json)).Throws(new Exception("fail"));

                var result = await Subject.HandlePost(QueryName, Json) as ObjectResult;

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            QueryName = "FakeQuery";

            async Task should_invoke_the_query_processor()
            {
                await Subject.HandleGet(QueryName);

                FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>()));
            }

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new object();
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleGet(QueryName) as OkObjectResult;

                result.Value.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandleGet(QueryName) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>())).Throws(new Exception("fail"));

                var result = await Subject.HandleGet(QueryName) as ObjectResult;

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        Mock<IQueryProcessor> FakeQueryProcessor;
        BaseQueryController Subject;
        string QueryName;
        JObject Json;
    }
}