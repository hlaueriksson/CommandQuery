using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.Tests._
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
                var fakeQueryHandler = new FakeQueryHandler(x => { Expected = x; return new FakeResult(); });
                FakeServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(fakeQueryHandler);

                var query = new FakeQuery();

                await Subject.ProcessAsync(query);

                Expected.Should().Be(query);
            }

            async Task should_return_the_result_from_the_query_handler()
            {
                var expected = new FakeResult();
                var query = new FakeQuery();

                var fakeQueryHandler = new FakeQueryHandler(x => expected);
                FakeServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(fakeQueryHandler);

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

                Subject.Awaiting(async x => await x.ProcessAsync<object>(queryName, (IDictionary<string, string>)null)).Should()
                    .Throw<QueryProcessorException>()
                    .WithMessage("The dictionary could not be converted to an object");
            }
        }

        Mock<IQueryTypeCollection> FakeQueryTypeCollection;
        Mock<IServiceProvider> FakeServiceProvider;
        QueryProcessor Subject;
        FakeQuery Expected;
    }
}