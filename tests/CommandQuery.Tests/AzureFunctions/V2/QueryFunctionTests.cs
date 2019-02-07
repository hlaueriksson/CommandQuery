#if NETCOREAPP2_0
using System;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CommandQuery.Tests.AzureFunctions.V2
{
    public class QueryFunctionTests : LoFuTest<QueryFunction>
    {
        public async Task when_handling_the_query()
        {
            Req = new DefaultHttpRequest(new DefaultHttpContext());
            Log = Use<ILogger>();

            Use<Mock<IQueryProcessor>>();

            async Task should_handle_QueryValidationException()
            {
                var queryName = "FakeQuery";
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, It.IsAny<string>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(queryName, Req, Log) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                var queryName = "FakeQuery";
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(queryName, Req, Log) as ObjectResult;

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        DefaultHttpRequest Req;
        ILogger Log;
    }
}
#endif