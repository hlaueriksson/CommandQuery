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
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests
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
            Req = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Method = "POST",
                Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"))
            };

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(QueryName, Req, Logger) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }

            async Task should_handle_QueryProcessorException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.HandleAsync(QueryName, Req, Logger);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_QueryException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryException("invalid"));

                var result = await Subject.HandleAsync(QueryName, Req, Logger);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(QueryName, Req, Logger);

                result.ShouldBeError("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Req = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Method = "GET",
                Query = new QueryCollection(new Dictionary<string, StringValues> { { "foo", new StringValues("bar") } })
            };

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(QueryName, Req, Logger) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }
        }

        DefaultHttpRequest Req;
        ILogger Logger;
        string QueryName;
    }
}
