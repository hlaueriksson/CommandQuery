using System;
using CommandQuery.AzureFunctions;
using Machine.Fakes;
using Machine.Specifications;
using Newtonsoft.Json.Linq;

namespace CommandQuery.Specs.AzureFunctions
{
    public class QueryFunctionSpecs
    {
        [Subject(typeof(QueryFunction))]
        public class when_handling_the_query : WithSubject<QueryFunction>
        {
            It should_invoke_the_query_processor = () =>
            {
                var queryName = "FakeQuery";
                var content = "{}";

                Subject.Handle(queryName, content).Await();

                The<IQueryProcessor>().WasToldTo(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<JObject>()));
            };

            It should_not_handle_Exception = () =>
            {
                var queryName = "FakeQuery";
                var content = "{}";

                The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<JObject>())).Throw(new Exception("fail"));

                Catch.Exception(() => Subject.Handle(queryName, content).Await()).ShouldBeOfExactType<Exception>();
            };
        }
    }
}