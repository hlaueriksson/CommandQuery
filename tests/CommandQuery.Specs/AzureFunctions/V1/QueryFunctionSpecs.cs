#if NET461
using System;
using System.Net.Http;
using System.Web.Http;
using CommandQuery.AzureFunctions;
using CommandQuery.Exceptions;
using Machine.Fakes;
using Machine.Specifications;

namespace CommandQuery.Specs.AzureFunctions.V1
{
    public class QueryFunctionSpecs
    {
        [Subject(typeof(QueryFunction))]
        public class when_handling_the_query : WithSubject<QueryFunction>
        {
            Establish context = () =>
            {
                Req = new HttpRequestMessage { Method = HttpMethod.Post, Content = new StringContent("") };
                Req.SetConfiguration(new HttpConfiguration());
                Log = new FakeTraceWriter();
            };

            It should_handle_QueryValidationException = () =>
            {
                var queryName = "FakeQuery";
                The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<string>())).Throw(new QueryValidationException("invalid"));

                var result = Subject.Handle(queryName, Req, Log).Result;

                result.ShouldBeError("invalid");
            };

            It should_handle_Exception = () =>
            {
                var queryName = "FakeQuery";
                The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<string>())).Throw(new Exception("fail"));

                var result = Subject.Handle(queryName, Req, Log).Result;

                result.ShouldBeError("fail");
            };

            static HttpRequestMessage Req;
            static FakeTraceWriter Log;
        }
    }
}
#endif