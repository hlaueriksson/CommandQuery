using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.NewtonsoftJson;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests.NewtonsoftJson
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
                FakeQueryProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeComplexQuery>())).Returns(Task.FromResult(Enumerable.Empty<FakeResult>()));

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

                FakeQueryProcessor.Verify(x => x.ProcessAsync(It.Is<FakeComplexQuery>(y =>
                    y.String == "Value" &&
                    y.Int == 1 &&
                    y.Bool &&
                    y.DateTime == DateTime.Parse("2018-07-06") &&
                    y.Guid == new Guid("3B10C34C-D423-4EC3-8811-DA2E0606E241") &&
                    y.NullableDouble == 2.1 &&
                    y.Array.SequenceEqual(new[] { 1, 2 }) &&
                    y.IEnumerable.SequenceEqual(new[] { 3, 4 }) &&
                    y.List.SequenceEqual(new[] { 5, 6 }))));
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
