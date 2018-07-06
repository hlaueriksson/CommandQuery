using System;
using System.Collections.Generic;
using CommandQuery.AzureFunctions;
using Machine.Fakes;
using Machine.Specifications;

namespace CommandQuery.Specs.AzureFunctions
{
    public class QueryFunctionSpecs
    {
        [Subject(typeof(QueryFunction))]
        public class when_handling_the_query : WithSubject<QueryFunction>
        {
            public class method_Post
            {
                It should_invoke_the_query_processor = () =>
                {
                    var queryName = "FakeQuery";
                    var content = "{}";

                    Subject.Handle(queryName, content).Await();

                    The<IQueryProcessor>().WasToldTo(x => x.ProcessAsync<object>(queryName, content));
                };

                It base_method_should_not_handle_Exception = () =>
                {
                    var queryName = "FakeQuery";
                    var content = "{}";

                    The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, content)).Throw(new Exception("fail"));

                    Catch.Exception(() => Subject.Handle(queryName, content).Await()).ShouldBeOfExactType<Exception>();
                };
            }

            public class method_Get
            {
                It should_invoke_the_query_processor = () =>
                {
                    var queryName = "FakeQuery";
                    var query = new Dictionary<string, string>();

                    Subject.Handle(queryName, query).Await();

                    The<IQueryProcessor>().WasToldTo(x => x.ProcessAsync<object>(queryName, query));
                };

                It base_method_should_not_handle_Exception = () =>
                {
                    var queryName = "FakeQuery";
                    var query = new Dictionary<string, string>();

                    The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, query)).Throw(new Exception("fail"));

                    Catch.Exception(() => Subject.Handle(queryName, query).Await()).ShouldBeOfExactType<Exception>();
                };
            }
        }
    }
}