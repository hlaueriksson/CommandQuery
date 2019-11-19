using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.AspNet.WebApi.Tests
{
    public class BaseQueryControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            FakeQueryProcessor = new Mock<IQueryProcessor>();
            Subject = new FakeQueryController(FakeQueryProcessor.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
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

                var result = await Subject.HandlePost(QueryName, Json) as OkNegotiatedContentResult<object>;

                result.Content.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, Json)).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandlePost(QueryName, Json) as BadRequestErrorMessageResult;

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, Json)).Throws(new Exception("fail"));

                var result = await Subject.HandlePost(QueryName, Json) as ExceptionResult;

                await result.ShouldBeErrorAsync("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            QueryName = "FakeQuery";

            async Task should_invoke_the_query_processor()
            {
                await Subject.HandleGet(QueryName);

                FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, IEnumerable<string>>>()));
            }

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new object();
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, IEnumerable<string>>>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleGet(QueryName) as OkNegotiatedContentResult<object>;

                result.Content.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, IEnumerable<string>>>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandleGet(QueryName) as BadRequestErrorMessageResult;

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, IEnumerable<string>>>())).Throws(new Exception("fail"));

                var result = await Subject.HandleGet(QueryName) as ExceptionResult;

                await result.ShouldBeErrorAsync("fail");
            }
        }

        Mock<IQueryProcessor> FakeQueryProcessor;
        BaseQueryController Subject;
        string QueryName;
        JObject Json;
    }
}