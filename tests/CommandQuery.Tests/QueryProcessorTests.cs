using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests
{
    public class QueryProcessorTests
    {
        [LoFu, Test]
        public async Task when_processing_the_query()
        {
            FakeQueryTypeCollection = new Mock<IQueryTypeCollection>();
            FakeServiceProvider = new Mock<IServiceProvider>();
            Subject = new QueryProcessor(FakeQueryTypeCollection.Object, FakeServiceProvider.Object);

            async Task should_invoke_the_correct_query_handler()
            {
                FakeQuery expectedQuery = null;
                var fakeQueryHandler = new FakeQueryHandler(x => { expectedQuery = x; return new FakeResult(); });
                FakeServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(fakeQueryHandler);

                var query = new FakeQuery();
                await Subject.ProcessAsync(query);

                query.Should().Be(expectedQuery);
            }

            async Task should_return_the_result_from_the_query_handler()
            {
                var expected = new FakeResult();
                var fakeQueryHandler = new FakeQueryHandler(x => expected);
                FakeServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(fakeQueryHandler);

                var query = new FakeQuery();
                var result = await Subject.ProcessAsync(query);

                result.Should().Be(expected);
            }

            void should_throw_exception_if_the_query_handler_is_not_found()
            {
                var query = new Mock<IQuery<FakeResult>>().Object;

                Subject.Awaiting(async x => await x.ProcessAsync(query)).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage($"The query handler for '{query}' could not be found");
            }
        }

        [LoFu, Test]
        public void when_get_query_types()
        {
            FakeQueryTypeCollection = new Mock<IQueryTypeCollection>();
            Subject = new QueryProcessor(FakeQueryTypeCollection.Object, null);

            void should_get_all_types_from_the_cache()
            {
                Subject.GetQueryTypes();

                FakeQueryTypeCollection.Verify(x => x.GetTypes());
            }
        }

        [LoFu, Test]
        public void when_get_query_type()
        {
            FakeQueryTypeCollection = new Mock<IQueryTypeCollection>();
            Subject = new QueryProcessor(FakeQueryTypeCollection.Object, null);

            void should_get_the_type_from_the_cache()
            {
                var queryName = "name";

                Subject.GetQueryType(queryName);

                FakeQueryTypeCollection.Verify(x => x.GetType(queryName));
            }
        }

        Mock<IQueryTypeCollection> FakeQueryTypeCollection;
        Mock<IServiceProvider> FakeServiceProvider;
        QueryProcessor Subject;
    }
}