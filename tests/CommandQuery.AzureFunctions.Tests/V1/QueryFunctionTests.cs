﻿#if NET472
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
            _log = new FakeTraceWriter();
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

                var result = await Subject.Handle(QueryName, Req, _log);

                result.IsSuccessStatusCode.Should().BeTrue();
                result.Content.Should().NotBeNull();
            }

            async Task should_handle_QueryValidationException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<string>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(QueryName, Req, _log);

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(QueryName, Req, _log);

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
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, string>>())).Returns(Task.FromResult(new object()));

                var result = await Subject.Handle(QueryName, Req, _log);

                result.IsSuccessStatusCode.Should().BeTrue();
                result.Content.Should().NotBeNull();
            }

            async Task should_handle_QueryValidationException()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, string>>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(QueryName, Req, _log);

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(QueryName, It.IsAny<IDictionary<string, string>>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(QueryName, Req, _log);

                await result.ShouldBeErrorAsync("fail");
            }
        }

        HttpRequestMessage Req;
        FakeTraceWriter _log;
        string QueryName;
    }
}
#endif