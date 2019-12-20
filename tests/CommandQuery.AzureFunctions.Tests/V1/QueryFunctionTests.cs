#if NET472
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V1
{
    public class QueryFunctionTests : LoFuTest<QueryFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            Use<Mock<IQueryProcessor>>();
            Logger = new FakeTraceWriter();
            QueryName = "FakeQuery";
            The<Mock<IQueryProcessor>>().Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Req = new HttpRequestMessage { Method = HttpMethod.Post, Content = new StringContent("{}") };
            Req.SetConfiguration(new HttpConfiguration());

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var result = await Subject.Handle(QueryName, Req, Logger);

                result.StatusCode.Should().Be(200);
                result.Content.ReadAsAsync<FakeQuery>().Should().NotBeNull();
            }

            async Task should_handle_QueryProcessorException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.Handle(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("fail", 400);
            }

            async Task should_handle_QueryValidationException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Req = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = new Uri("http://example.com?foo=bar") };
            Req.SetConfiguration(new HttpConfiguration());

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new FakeResult();
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var result = await Subject.Handle(QueryName, Req, Logger);

                result.StatusCode.Should().Be(200);
                result.Content.ReadAsAsync<FakeQuery>().Should().NotBeNull();
            }
        }

        HttpRequestMessage Req;
        FakeTraceWriter Logger;
        string QueryName;
    }
}
#endif