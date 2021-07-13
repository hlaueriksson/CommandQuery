#if NET5_0
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Serialization;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V5
{
    public class QueryFunctionTests : LoFuTest<QueryFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            Use<Mock<IQueryProcessor>>();
            Use<JsonSerializerOptions>(null);
            Logger = Use<ILogger>();
            QueryName = "FakeQuery";
            The<Mock<IQueryProcessor>>().Setup(x => x.GetQueryType(QueryName)).Returns(typeof(FakeQuery));

            var options = new Mock<IOptions<WorkerOptions>>();
            options.Setup(x => x.Value).Returns(new WorkerOptions { Serializer = new JsonObjectSerializer() });
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IOptions<WorkerOptions>))).Returns(options.Object);
            Context = new Mock<FunctionContext>();
            Context.Setup(x => x.InstanceServices).Returns(serviceProvider.Object);
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

                var result = await Subject.HandleAsync(QueryName, Req, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Length.Should().BeGreaterThan(0);
            }

            async Task should_throw_when_request_is_null()
            {
                Subject.Awaiting(x => x.HandleAsync(QueryName, null, Logger))
                    .Should().Throw<ArgumentNullException>();
            }

            async Task should_handle_QueryProcessorException()
            {
                Req.Body.Position = 0;
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryProcessorException("fail"));

                var result = await Subject.HandleAsync(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("fail", 400);
            }

            async Task should_handle_QueryException()
            {
                Req.Body.Position = 0;
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new QueryException("invalid"));

                var result = await Subject.HandleAsync(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                Req.Body.Position = 0;
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(QueryName, Req, Logger);

                await result.ShouldBeErrorAsync("fail", 500);
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

                var result = await Subject.HandleAsync(QueryName, Req, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Length.Should().BeGreaterThan(0);
            }
        }

        HttpRequestData Req;
        ILogger Logger;
        string QueryName;
        Mock<FunctionContext> Context;
    }
}
#endif
