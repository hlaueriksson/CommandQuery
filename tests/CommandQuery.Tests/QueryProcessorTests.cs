using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using Newtonsoft.Json.Linq;
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

            async Task should_create_the_query_from_a_string()
            {
                var expectedQueryType = typeof(FakeQuery);
                var fakeQueryHandler = new Mock<IQueryHandler<FakeQuery, FakeResult>>();
                FakeQueryTypeCollection.Setup(x => x.GetType(expectedQueryType.Name)).Returns(expectedQueryType);
                FakeServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(fakeQueryHandler.Object);

                await Subject.ProcessAsync<FakeResult>(expectedQueryType.Name, "{}");

                fakeQueryHandler.Verify(x => x.HandleAsync(It.IsAny<FakeQuery>()));
            }

            async Task should_create_the_query_from_a_dictionary()
            {
                var expectedQueryType = typeof(FakeQuery);
                var fakeQueryHandler = new Mock<IQueryHandler<FakeQuery, FakeResult>>();
                FakeQueryTypeCollection.Setup(x => x.GetType(expectedQueryType.Name)).Returns(expectedQueryType);
                FakeServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(fakeQueryHandler.Object);

                await Subject.ProcessAsync<FakeResult>(expectedQueryType.Name, new Dictionary<string, IEnumerable<string>>());

                fakeQueryHandler.Verify(x => x.HandleAsync(It.IsAny<FakeQuery>()));
            }

            async Task should_create_a_complex_query_from_a_dictionary()
            {
                var expectedQueryType = typeof(FakeComplexQuery);
                FakeComplexQuery expectedQuery = null;
                var fakeQueryHandler = new FakeComplexQueryHandler(x => { expectedQuery = x; return new List<FakeResult>(); });
                FakeQueryTypeCollection.Setup(x => x.GetType(expectedQueryType.Name)).Returns(expectedQueryType);
                FakeServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<FakeComplexQuery, IEnumerable<FakeResult>>))).Returns(fakeQueryHandler);

                var query = new Dictionary<string, IEnumerable<string>>
                {
                    {"String", new[] {"Value"}},
                    {"Int", new[] {"1"}},
                    {"Bool", new[] {"true"}},
                    {"DateTime", new[] {"2018-07-06"}},
                    {"Guid", new[] {"3B10C34C-D423-4EC3-8811-DA2E0606E241"}},
                    {"NullableDouble", new[] {"2.1"}},
                    {"UndefinedProperty", new[] {"should_not_be_used"}},
                    {"Array", new[] {"1", "2"}},
                    {"IEnumerable", new[] {"3", "4"}},
                    {"List", new[] {"5", "6"}}
                };

                await Subject.ProcessAsync<IEnumerable<FakeResult>>(expectedQueryType.Name, query);

                expectedQuery.String.Should().Be("Value");
                expectedQuery.Int.Should().Be(1);
                expectedQuery.Bool.Should().Be(true);
                expectedQuery.DateTime.Should().Be(DateTime.Parse("2018-07-06"));
                expectedQuery.Guid.Should().Be(new Guid("3B10C34C-D423-4EC3-8811-DA2E0606E241"));
                expectedQuery.NullableDouble.Should().Be(2.1);
                expectedQuery.Array.Should().Equal(1, 2);
                expectedQuery.IEnumerable.Should().Equal(3, 4);
                expectedQuery.List.Should().Equal(5, 6);
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

            void should_throw_exception_if_the_query_type_is_not_found()
            {
                var queryName = "NotFoundQuery";
                var json = JObject.Parse("{}");

                Subject.Awaiting(async x => await x.ProcessAsync<object>(queryName, json)).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage("The query type 'NotFoundQuery' could not be found");
            }

            void should_throw_exception_if_the_json_is_invalid()
            {
                var queryName = "FakeQuery";
                FakeQueryTypeCollection.Setup(x => x.GetType(queryName)).Returns(typeof(FakeQuery));

                Subject.Awaiting(async x => await x.ProcessAsync<object>(queryName, (JObject)null)).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage("The json could not be converted to an object");
            }

            void should_throw_exception_if_the_dictionary_is_invalid()
            {
                var queryName = "FakeQuery";
                FakeQueryTypeCollection.Setup(x => x.GetType(queryName)).Returns(typeof(FakeQuery));

                Subject.Awaiting(async x => await x.ProcessAsync<object>(queryName, (IDictionary<string, IEnumerable<string>>)null)).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage("The dictionary could not be converted to an object");
            }
        }

        [LoFu, Test]
        public void when_get_queries()
        {
            FakeQueryTypeCollection = new Mock<IQueryTypeCollection>();
            Subject = new QueryProcessor(FakeQueryTypeCollection.Object, null);

            void should_get_all_types_from_the_cache()
            {
                Subject.GetQueries();

                FakeQueryTypeCollection.Verify(x => x.GetTypes());
            }
        }

        Mock<IQueryTypeCollection> FakeQueryTypeCollection;
        Mock<IServiceProvider> FakeServiceProvider;
        QueryProcessor Subject;
    }
}