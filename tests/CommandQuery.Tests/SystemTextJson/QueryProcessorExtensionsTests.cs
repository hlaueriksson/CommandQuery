using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.SystemTextJson;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests.SystemTextJson
{
    public class QueryProcessorExtensionsTests
    {
        [LoFu, Test]
        public async Task when_processing_the_query_from_json()
        {
            FakeQueryProcessor = new Mock<IQueryProcessor>();
            Subject = FakeQueryProcessor.Object;

            async Task should_create_the_query_from_a_json_string()
            {
                var expectedQueryType = typeof(FakeQuery);
                FakeQueryProcessor.Setup(x => x.GetQueryType(expectedQueryType.Name)).Returns(expectedQueryType);
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(new FakeResult()));

                await Subject.ProcessAsync<FakeResult>(expectedQueryType.Name, "{}");

                FakeQueryProcessor.Verify(x => x.ProcessAsync(It.IsAny<FakeQuery>()));
            }

            void should_throw_exception_if_the_IQueryProcessor_is_null()
            {
                Subject.Awaiting(x => ((IQueryProcessor)null).ProcessAsync<object>("", "{}")).Should()
                    .Throw<ArgumentNullException>();
            }

            void should_throw_exception_if_the_query_type_is_not_found_for_the_json()
            {
                var queryName = "NotFoundQuery";

                Subject.Awaiting(x => x.ProcessAsync<object>(queryName, "{}")).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage("The query type 'NotFoundQuery' could not be found");
            }

            void should_throw_exception_if_the_json_is_null()
            {
                var queryName = "FakeQuery";

                Subject.Awaiting(x => x.ProcessAsync<object>(queryName, (string)null)).Should()
                    .Throw<ArgumentNullException>()
                    .WithMessage("Value cannot be null*json*");
            }

            void should_throw_exception_if_the_json_is_invalid()
            {
                var queryName = "FakeQuery";

                Subject.Awaiting(x => x.ProcessAsync<object>(queryName, "<>")).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage("The json string could not be deserialized to an object");
            }
        }

        [LoFu, Test]
        public async Task when_processing_the_query_from_dictionary()
        {
            FakeQueryProcessor = new Mock<IQueryProcessor>();
            Subject = FakeQueryProcessor.Object;

            async Task should_create_the_query_from_a_dictionary()
            {
                var expectedQueryType = typeof(FakeQuery);
                FakeQueryProcessor.Setup(x => x.GetQueryType(expectedQueryType.Name)).Returns(expectedQueryType);

                await Subject.ProcessAsync<FakeResult>(expectedQueryType.Name, new Dictionary<string, IEnumerable<string>>());

                FakeQueryProcessor.Verify(x => x.ProcessAsync(It.IsAny<FakeQuery>()));
            }

            async Task should_throw_exception_if_the_IQueryProcessor_is_null()
            {
                Subject.Awaiting(x => ((IQueryProcessor)null).ProcessAsync<object>("", new Dictionary<string, IEnumerable<string>>())).Should()
                    .Throw<ArgumentNullException>();
            }

            async Task should_throw_exception_if_the_query_type_is_not_found_for_the_dictionary()
            {
                var queryName = "NotFoundQuery";

                Subject.Awaiting(x => x.ProcessAsync<object>(queryName, new Dictionary<string, IEnumerable<string>>())).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage("The query type 'NotFoundQuery' could not be found");
            }

            async Task should_create_a_complex_query_from_a_dictionary()
            {
                var expectedQueryType = typeof(FakeComplexQuery);
                FakeQueryProcessor.Setup(x => x.GetQueryType(expectedQueryType.Name)).Returns(expectedQueryType);
                FakeComplexQuery actual = null;
                FakeQueryProcessor
                    .Setup(x => x.ProcessAsync(It.IsAny<FakeComplexQuery>()))
                    .Returns(Task.FromResult(Enumerable.Empty<FakeResult>()))
                    .Callback<FakeComplexQuery>(y => actual = y);

                var query = TestData.FakeComplexQuery_As_Dictionary_Of_String_IEnumerable_String;

                await Subject.ProcessAsync<IEnumerable<FakeResult>>(expectedQueryType.Name, query);

                actual.Should().BeEquivalentTo(TestData.FakeComplexQuery);
            }

            async Task should_create_a_query_with_DateTime_kinds_from_a_dictionary()
            {
                var expectedQueryType = typeof(FakeDateTimeQuery);
                FakeQueryProcessor.Setup(x => x.GetQueryType(expectedQueryType.Name)).Returns(expectedQueryType);
                FakeDateTimeQuery actual = null;
                FakeQueryProcessor
                    .Setup(x => x.ProcessAsync(It.IsAny<FakeDateTimeQuery>()))
                    .Returns(Task.FromResult(new FakeResult()))
                    .Callback<FakeDateTimeQuery>(y => actual = y);

                var query = TestData.FakeDateTimeQuery_As_Dictionary_Of_String_IEnumerable_String;

                await Subject.ProcessAsync<FakeResult>(expectedQueryType.Name, query);

                actual.Should().BeEquivalentTo(TestData.FakeDateTimeQuery);
            }

            async Task should_not_create_a_query_with_nested_objects_from_a_dictionary()
            {
                var expectedQueryType = typeof(FakeNestedQuery);
                FakeQueryProcessor.Setup(x => x.GetQueryType(expectedQueryType.Name)).Returns(expectedQueryType);
                FakeNestedQuery actual = null;
                FakeQueryProcessor
                    .Setup(x => x.ProcessAsync(It.IsAny<FakeNestedQuery>()))
                    .Returns(Task.FromResult(new FakeResult()))
                    .Callback<FakeNestedQuery>(y => actual = y);

                var query = TestData.FakeNestedQuery_As_Dictionary_Of_String_IEnumerable_String;

                await Subject.ProcessAsync<FakeResult>(expectedQueryType.Name, query);

                actual.Should().NotBeEquivalentTo(TestData.FakeNestedQuery);
            }

            void should_throw_exception_if_the_dictionary_is_invalid()
            {
                var queryName = "FakeQuery";

                Subject.Awaiting(x => x.ProcessAsync<object>(queryName, (IDictionary<string, IEnumerable<string>>)null)).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage("The dictionary could not be deserialized to an object");
            }
        }

        Mock<IQueryProcessor> FakeQueryProcessor;
        IQueryProcessor Subject;
    }
}
