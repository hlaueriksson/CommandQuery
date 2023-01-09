using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Tracing;
using CommandQuery.Exceptions;
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

                var result = await Subject.HandlePost(QueryName, Json) as OkNegotiatedContentResult<object>;

                (await result.ExecuteAsync(CancellationToken.None)).StatusCode.Should().Be(HttpStatusCode.OK);
                result.Content.Should().Be(expected);
            }

            async Task should_handle_QueryProcessorException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.HandlePost(QueryName, Json);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.BadRequest);
            }

            async Task should_handle_QueryException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryException("invalid"));

                var result = await Subject.HandlePost(QueryName, Json);

                await result.ShouldBeErrorAsync("invalid", HttpStatusCode.BadRequest);
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                var result = await Subject.HandlePost(QueryName, Json);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.InternalServerError);
            }

            async Task should_log_errors()
            {
                var fakeTraceWriter = new Mock<ITraceWriter>();
                var subject = new FakeQueryController(FakeQueryProcessor.Object, fakeTraceWriter.Object)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };

                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                await subject.HandlePost(QueryName, Json);

                fakeTraceWriter.Verify(x => x.Trace(It.IsAny<HttpRequestMessage>(), "UnhandledQueryException", TraceLevel.Error, It.IsAny<Action<TraceRecord>>()));
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Subject.Request.RequestUri = new Uri("http://example.com?foo=bar");
            QueryName = "FakeQuery";
            FakeQueryProcessor.Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleGet(QueryName) as OkNegotiatedContentResult<object>;

                (await result.ExecuteAsync(CancellationToken.None)).StatusCode.Should().Be(HttpStatusCode.OK);
                result.Content.Should().Be(expected);
            }

            async Task should_handle_QueryProcessorException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.HandleGet(QueryName);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.BadRequest);
            }

            async Task should_handle_QueryException()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryException("invalid"));

                var result = await Subject.HandleGet(QueryName);

                await result.ShouldBeErrorAsync("invalid", HttpStatusCode.BadRequest);
            }

            async Task should_handle_Exception()
            {
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                var result = await Subject.HandleGet(QueryName);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.InternalServerError);
            }
        }

        Mock<IQueryProcessor> FakeQueryProcessor;
        BaseQueryController Subject;
        string QueryName;
        JObject Json;
    }
}
