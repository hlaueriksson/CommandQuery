using System.Text;
using System.Text.Json;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;

namespace CommandQuery.GoogleCloudFunctions.Tests
{
    public class QueryFunctionTests : LoFuTest<QueryFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            QueryName = "FakeQuery";
            Use<Mock<IQueryProcessor>>();
            Use<JsonSerializerOptions>(null);
            The<Mock<IQueryProcessor>>().Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Context = new DefaultHttpContext();
            Context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            Context.Response.Body = new MemoryStream();

            async Task should_return_the_result_from_the_query_processor()
            {
                Context.Request.Body.Position = 0;
                Context.Response.Clear();
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                await Subject.HandleAsync(QueryName, Context);

                Context.Response.StatusCode.Should().Be(200);
                Context.Response.Body.Length.Should().BeGreaterThan(0);
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(QueryName, null);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_QueryProcessorException()
            {
                Context.Request.Body.Position = 0;
                Context.Response.Clear();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryProcessorException("fail"));

                await Subject.HandleAsync(QueryName, Context);

                await Context.Response.ShouldBeErrorAsync("fail", 400);
            }

            async Task should_handle_QueryException()
            {
                Context.Request.Body.Position = 0;
                Context.Response.Clear();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryException("invalid"));

                await Subject.HandleAsync(QueryName, Context);

                await Context.Response.ShouldBeErrorAsync("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                Context.Request.Body.Position = 0;
                Context.Response.Clear();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                await Subject.HandleAsync(QueryName, Context);

                await Context.Response.ShouldBeErrorAsync("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Context = new DefaultHttpContext();
            Context.Request.Method = "GET";
            Context.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { { "foo", new StringValues("bar") } });
            Context.Response.Body = new MemoryStream();

            async Task should_return_the_result_from_the_query_processor()
            {
                Context.Response.Clear();
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                await Subject.HandleAsync(QueryName, Context);

                Context.Response.StatusCode.Should().Be(200);
                Context.Response.Body.Length.Should().BeGreaterThan(0);
            }
        }

        HttpContext Context;
        string QueryName;
    }
}
