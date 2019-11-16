#if NET472
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CommandQuery.Exceptions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V1
{
    public class QueryFunctionTests : LoFuTest<QueryFunction>
    {
        [LoFu, Test]
        public async Task when_handling_the_query()
        {
            _req = new HttpRequestMessage { Method = HttpMethod.Post, Content = new StringContent("") };
            _req.SetConfiguration(new HttpConfiguration());
            _log = new FakeTraceWriter();

            Use<Mock<IQueryProcessor>>();

            async Task should_handle_QueryValidationException()
            {
                var queryName = "FakeQuery";
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, It.IsAny<string>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(queryName, _req, _log);

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                var queryName = "FakeQuery";
                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(queryName, _req, _log);

                await result.ShouldBeErrorAsync("fail");
            }
        }

        HttpRequestMessage _req;
        FakeTraceWriter _log;
    }
}
#endif