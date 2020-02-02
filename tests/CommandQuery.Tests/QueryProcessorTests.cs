using System;
using System.Collections.Generic;
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

            async Task should_invoke_the_correct_query_handler_and_return_a_result()
            {
                FakeQuery expectedQuery = null;
                var expectedResult = new FakeResult();
                var fakeQueryHandler = new FakeQueryHandler(x => { expectedQuery = x; return expectedResult; });
                FakeServiceProvider.Setup(x => x.GetService(typeof(IEnumerable<IQueryHandler<FakeQuery, FakeResult>>))).Returns(new[] { fakeQueryHandler });

                var query = new FakeQuery();
                var result = await Subject.ProcessAsync(query);

                query.Should().Be(expectedQuery);
                result.Should().Be(expectedResult);
            }

            void should_throw_exception_if_the_query_handler_is_not_found()
            {
                var query = new Mock<IQuery<FakeResult>>().Object;

                Subject.Awaiting(x => x.ProcessAsync(query)).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage($"The query handler for '{query}' could not be found");
            }

            void should_throw_exception_if_multiple_query_handlers_are_found()
            {
                var handlerType = typeof(IQueryHandler<FakeMultiQuery1, FakeResult>);
                var enumerableType = typeof(IEnumerable<IQueryHandler<FakeMultiQuery1, FakeResult>>);
                FakeServiceProvider.Setup(x => x.GetService(enumerableType)).Returns(new[] { new Mock<IQueryHandler<FakeMultiQuery1, FakeResult>>().Object, new Mock<IQueryHandler<FakeMultiQuery1, FakeResult>>().Object });

                var query = new FakeMultiQuery1();

                Subject.Awaiting(x => x.ProcessAsync(query)).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage($"Multiple query handlers for '{handlerType}' was found");
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