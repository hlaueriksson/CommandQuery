using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommandQuery.DependencyInjection;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests
{
    public class QueryProcessorTests
    {
        [LoFu, Test]
        public async Task when_processing_the_query()
        {
            FakeQueryTypeProvider = new Mock<IQueryTypeProvider>();
            FakeServiceProvider = new Mock<IServiceProvider>();
            Subject = new QueryProcessor(FakeQueryTypeProvider.Object, FakeServiceProvider.Object);

            async Task should_invoke_the_correct_query_handler_and_return_a_result()
            {
                FakeQuery expectedQuery = null;
                var expectedResult = new FakeResult();
                var fakeQueryHandler = new FakeQueryHandler
                {
                    Callback = x =>
                    {
                        expectedQuery = x;
                        return expectedResult;
                    }
                };
                FakeServiceProvider.Setup(x => x.GetService(typeof(IEnumerable<IQueryHandler<FakeQuery, FakeResult>>))).Returns(new[] { fakeQueryHandler });

                var query = new FakeQuery();
                var result = await Subject.ProcessAsync(query);

                query.Should().Be(expectedQuery);
                result.Should().Be(expectedResult);
            }

            async Task should_throw_exception_if_the_query_is_null()
            {
                Func<Task> act = () => Subject.ProcessAsync<object>(null);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_throw_exception_if_the_query_handler_is_not_found()
            {
                var query = new Mock<IQuery<FakeResult>>().Object;

                Func<Task> act = () => Subject.ProcessAsync(query);
                await act.Should().ThrowAsync<QueryProcessorException>()
                    .WithMessage($"The query handler for '{query}' could not be found.");
            }

            async Task should_throw_exception_if_multiple_query_handlers_are_found()
            {
                var handlerType = typeof(IQueryHandler<FakeMultiQuery1, FakeResult>);
                var enumerableType = typeof(IEnumerable<IQueryHandler<FakeMultiQuery1, FakeResult>>);
                FakeServiceProvider.Setup(x => x.GetService(enumerableType)).Returns(new[] { new Mock<IQueryHandler<FakeMultiQuery1, FakeResult>>().Object, new Mock<IQueryHandler<FakeMultiQuery1, FakeResult>>().Object });

                var query = new FakeMultiQuery1();

                Func<Task> act = () => Subject.ProcessAsync(query);
                await act.Should().ThrowAsync<QueryProcessorException>()
                    .WithMessage($"A single query handler for '{handlerType}' could not be retrieved.");
            }
        }

        [LoFu, Test]
        public void when_get_query_types()
        {
            FakeQueryTypeProvider = new Mock<IQueryTypeProvider>();
            Subject = new QueryProcessor(FakeQueryTypeProvider.Object, null);

            void should_get_all_types_from_the_cache()
            {
                Subject.GetQueryTypes();

                FakeQueryTypeProvider.Verify(x => x.GetQueryTypes());
            }
        }

        [LoFu, Test]
        public void when_get_query_type()
        {
            FakeQueryTypeProvider = new Mock<IQueryTypeProvider>();
            Subject = new QueryProcessor(FakeQueryTypeProvider.Object, null);

            void should_get_the_type_from_the_cache()
            {
                var queryName = "name";

                Subject.GetQueryType(queryName);

                FakeQueryTypeProvider.Verify(x => x.GetQueryType(queryName));
            }
        }

        [Test]
        public void AssertConfigurationIsValid()
        {
            var subject = typeof(FakeQueryHandler).Assembly.GetQueryProcessor();

            subject.Invoking(x => x.AssertConfigurationIsValid())
                .Should().Throw<QueryTypeException>()
                .WithMessage("*The query handler for * is not registered.*")
                .WithMessage("*A single query handler for * could not be retrieved.*")
                .WithMessage("*The query * is not registered.*");

            new QueryProcessor(new QueryTypeProvider(), new ServiceCollection().BuildServiceProvider())
                .AssertConfigurationIsValid().Should().NotBeNull();
        }

        Mock<IQueryTypeProvider> FakeQueryTypeProvider;
        Mock<IServiceProvider> FakeServiceProvider;
        QueryProcessor Subject;
    }

    public class DupeQueryHandler : IQueryHandler<Fail.DupeQuery, object>
    {
        public async Task<object> HandleAsync(Fail.DupeQuery query, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
