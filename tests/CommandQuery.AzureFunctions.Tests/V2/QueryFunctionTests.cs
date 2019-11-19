#if NETCOREAPP2_2
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V2
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
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Req = new DefaultHttpRequest(new DefaultHttpContext()) { Method = "POST" };

            async Task should_invoke_the_query_processor()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<string>())).Returns(Task.FromResult(new object()));

                var result = await Subject.Handle(QueryName, Req, Logger) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Should().NotBeNull();
            }

            async Task should_handle_QueryValidationException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<string>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(QueryName, Req, Logger) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(QueryName, Req, Logger) as ObjectResult;

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Req = new DefaultHttpRequest(new DefaultHttpContext()) { Method = "GET" };

            async Task should_invoke_the_query_processor()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>())).Returns(Task.FromResult(new object()));

                var result = await Subject.Handle(QueryName, Req, Logger) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Should().NotBeNull();
            }

            async Task should_handle_QueryValidationException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(QueryName, Req, Logger) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(QueryName, Req, Logger) as ObjectResult;

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        DefaultHttpRequest Req;
        ILogger Logger;
        string QueryName;
    }
}
#endif