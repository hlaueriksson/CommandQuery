using System.Net;
using System.Text;
using System.Text.Json;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests
{
    public class QueryFunctionTests_HttpRequestData : LoFuTest<QueryFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            QueryName = "FakeQuery";
            Use<Mock<IQueryProcessor>>();
            Use<JsonSerializerOptions>(null);
            The<Mock<IQueryProcessor>>().Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));

            Context = new Mock<FunctionContext>();
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Req = new FakeHttpRequestData(Context.Object, "POST");
            await Req.Body.WriteAsync(Encoding.UTF8.GetBytes("{}"));

            async Task should_return_the_result_from_the_query_processor()
            {
                Req.Body.Position = 0;
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(QueryName, Req);

                result.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Body.Length.Should().BeGreaterThan(0);
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(QueryName, (HttpRequestData)null);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_QueryProcessorException()
            {
                Req.Body.Position = 0;
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.HandleAsync(QueryName, Req);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.BadRequest);
            }

            async Task should_handle_QueryException()
            {
                Req.Body.Position = 0;
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryException("invalid"));

                var result = await Subject.HandleAsync(QueryName, Req);

                await result.ShouldBeErrorAsync("invalid", HttpStatusCode.BadRequest);
            }

            async Task should_handle_Exception()
            {
                Req.Body.Position = 0;
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(QueryName, Req);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.InternalServerError);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Req = new FakeHttpRequestData(Context.Object, "GET", new Uri("http://localhost/api/query/FakeQuery?foo=bar"));

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(QueryName, Req);

                result.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Body.Length.Should().BeGreaterThan(0);
            }
        }

        HttpRequestData Req;
        string QueryName;
        Mock<FunctionContext> Context;
    }

    public class QueryFunctionTests_HttpRequest : LoFuTest<QueryFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            QueryName = "FakeQuery";
            Use<Mock<IQueryProcessor>>();
            Use<JsonSerializerOptions>(null);
            The<Mock<IQueryProcessor>>().Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));

            Context = new Mock<FunctionContext>();
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            var mock = new Mock<HttpRequest>();
            mock.Setup(x => x.Method).Returns("POST");
            mock.SetupGet(x => x.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes("{}")));
            Req = mock.Object;

            async Task should_return_the_result_from_the_query_processor()
            {
                Req.Body.Position = 0;
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(QueryName, Req);

                (result as IStatusCodeActionResult).StatusCode.Should().Be(200);
                (result as ObjectResult).Value.Should().NotBeNull();
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(QueryName, (HttpRequest)null);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_QueryProcessorException()
            {
                Req.Body.Position = 0;
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.HandleAsync(QueryName, Req);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_QueryException()
            {
                Req.Body.Position = 0;
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryException("invalid"));

                var result = await Subject.HandleAsync(QueryName, Req);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                Req.Body.Position = 0;
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(QueryName, Req);

                result.ShouldBeError("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            var mock = new Mock<HttpRequest>();
            mock.SetupGet(x => x.Method).Returns("GET");
            mock.SetupGet(x => x.Query).Returns(new QueryCollection());
            Req = mock.Object;

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(QueryName, Req);

                (result as IStatusCodeActionResult).StatusCode.Should().Be(200);
                (result as ObjectResult).Value.Should().NotBeNull();
            }
        }

        HttpRequest Req;
        string QueryName;
        Mock<FunctionContext> Context;
    }
}
