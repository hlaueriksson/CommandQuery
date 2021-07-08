using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
            Use<Mock<IQueryProcessor>>();
            Logger = Use<ILogger>();
            QueryName = "FakeQuery";
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
                Context.Response.Clear();
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                await Subject.HandleAsync(QueryName, Context, Logger);

                Context.Response.StatusCode.Should().Be(200);
                Context.Response.Body.Length.Should().BeGreaterThan(0);
            }

            async Task should_throw_when_request_is_null()
            {
                Subject.Awaiting(x => x.HandleAsync(QueryName, null, Logger))
                    .Should().Throw<ArgumentNullException>();
            }

            async Task should_handle_QueryProcessorException()
            {
                Context.Response.Clear();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryProcessorException("fail"));

                await Subject.HandleAsync(QueryName, Context, Logger);

                await Context.Response.ShouldBeErrorAsync("fail", 400);
            }

            async Task should_handle_QueryException()
            {
                Context.Response.Clear();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryException("invalid"));

                await Subject.HandleAsync(QueryName, Context, Logger);

                await Context.Response.ShouldBeErrorAsync("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                Context.Response.Clear();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                await Subject.HandleAsync(QueryName, Context, Logger);

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
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                await Subject.HandleAsync(QueryName, Context, Logger);

                Context.Response.StatusCode.Should().Be(200);
                Context.Response.Body.Length.Should().BeGreaterThan(0);
            }
        }

        HttpContext Context;
        ILogger Logger;
        string QueryName;
    }
}
