using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Machine.Specifications;
using Moq;
using Newtonsoft.Json.Linq;
using It = Machine.Specifications.It;

namespace CommandQuery.Specs
{
    public class QueryProcessorSpecs
    {
        [Subject(typeof(QueryProcessor))]
        public class when_processing_the_query
        {
            Establish context = () =>
            {
                FakeQueryTypeCollection = new Mock<IQueryTypeCollection>();
                FakeServiceProvider = new Mock<IServiceProvider>();
                Subject = new QueryProcessor(FakeQueryTypeCollection.Object, FakeServiceProvider.Object);
            };

            It should_invoke_the_correct_query_handler = async () =>
            {
                var query = new FakeQuery();
                var fakeQueryHandler = new Mock<IQueryHandler<FakeQuery, FakeResult>>();
                FakeServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(fakeQueryHandler.Object);

                await Subject.ProcessAsync(query);

                fakeQueryHandler.Verify(x => x.HandleAsync(query));
            };

            It should_return_the_result_from_the_query_handler = async () =>
            {
                var expected = new FakeResult();

                var query = new FakeQuery();
                var fakeQueryHandler = new Mock<IQueryHandler<FakeQuery, FakeResult>>();
                fakeQueryHandler.Setup(x => x.HandleAsync(query)).Returns(Task.FromResult(expected));
                FakeServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(fakeQueryHandler.Object);

                var result = await Subject.ProcessAsync(query);

                result.ShouldEqual(expected);
            };

            It should_throw_exception_if_the_query_handler_is_not_found = () =>
            {
                var query = new Mock<IQuery<FakeResult>>().Object;

                var exception = Catch.Exception(() => Subject.ProcessAsync(query).Await());

                exception.ShouldContainErrorMessage($"The query handler for '{query}' could not be found");
            };

            It should_throw_exception_if_the_query_type_is_not_found = () =>
            {
                var queryName = "NotFoundQuery";
                var json = JObject.Parse("{}");

                var exception = Catch.Exception(() => Subject.ProcessAsync<object>(queryName, json).Await());

                exception.ShouldContainErrorMessage("The query type 'NotFoundQuery' could not be found");
            };

            It should_throw_exception_if_the_json_is_invalid = () =>
            {
                var queryName = "FakeQuery";
                FakeQueryTypeCollection.Setup(x => x.GetType(queryName)).Returns(typeof(FakeQuery));

                var exception = Catch.Exception(() => Subject.ProcessAsync<object>(queryName, (JObject)null).Await());

                exception.ShouldContainErrorMessage("The json could not be converted to an object");
            };

            It should_throw_exception_if_the_dictionary_is_invalid = () =>
            {
                var queryName = "FakeQuery";
                FakeQueryTypeCollection.Setup(x => x.GetType(queryName)).Returns(typeof(FakeQuery));

                var exception = Catch.Exception(() => Subject.ProcessAsync<object>(queryName, (IDictionary<string, string>)null).Await());

                exception.ShouldContainErrorMessage("The dictionary could not be converted to an object");
            };

            static Mock<IQueryTypeCollection> FakeQueryTypeCollection;
            static Mock<IServiceProvider> FakeServiceProvider;
            static QueryProcessor Subject;
        }
    }
}