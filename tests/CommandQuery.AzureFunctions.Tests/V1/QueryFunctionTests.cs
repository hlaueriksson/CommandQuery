#if NET472
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using Newtonsoft.Json.Linq;
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
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Req = new HttpRequestMessage { Method = HttpMethod.Post, Content = new StringContent("") };
            Req.SetConfiguration(new HttpConfiguration());

            async Task should_invoke_the_query_processor()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<string>())).Returns(Task.FromResult(new object()));

                var result = await Subject.Handle(QueryName, Req, Logger);

                result.IsSuccessStatusCode.Should().BeTrue();
                result.Content.Should().NotBeNull();
            }

            async Task should_handle_QueryValidationException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<string>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("fail");
            }
        }    
        
        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Req = new HttpRequestMessage { Method = HttpMethod.Get };
            Req.SetConfiguration(new HttpConfiguration());

            async Task should_invoke_the_query_processor()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>())).Returns(Task.FromResult(new object()));

                var result = await Subject.Handle(QueryName, Req, Logger);

                result.IsSuccessStatusCode.Should().BeTrue();
                result.Content.Should().NotBeNull();
            }

            async Task should_handle_QueryValidationException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, JToken>>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("fail");
            }
        }

        HttpRequestMessage Req;
        FakeTraceWriter Logger;
        string QueryName;
    }
}
#endif