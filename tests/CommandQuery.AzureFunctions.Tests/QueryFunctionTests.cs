using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests
{
    public class QueryFunctionTests : LoFuTest<QueryFunction>
    {
        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            Use<Mock<IQueryProcessor>>();

            async Task should_invoke_the_query_processor()
            {
                var queryName = "FakeQuery";
                var content = "{}";

                await Subject.Handle(queryName, content);

                The<Mock<IQueryProcessor>>().Verify(x => x.ProcessAsync<object>(queryName, content));
            }

            async Task base_method_should_not_handle_Exception()
            {
                var queryName = "FakeQuery";
                var content = "{}";

                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, content)).Throws(new Exception("fail"));

                Subject.Awaiting(async x => await x.Handle(queryName, content)).Should()
                    .Throw<Exception>()
                    .WithMessage("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            Use<Mock<IQueryProcessor>>();

            async Task should_invoke_the_query_processor()
            {
                var queryName = "FakeQuery";
                var query = new Dictionary<string, string>();

                await Subject.Handle(queryName, query);

                The<Mock<IQueryProcessor>>().Verify(x => x.ProcessAsync<object>(queryName, query));
            }

            async Task base_method_should_not_handle_Exception()
            {
                var queryName = "FakeQuery";
                var query = new Dictionary<string, string>();

                The<Mock<IQueryProcessor>>().Setup(x => x.ProcessAsync<object>(queryName, query)).Throws(new Exception("fail"));

                Subject.Awaiting(async x => await x.Handle(queryName, query)).Should()
                    .Throw<Exception>()
                    .WithMessage("fail");
            }
        }
    }
}