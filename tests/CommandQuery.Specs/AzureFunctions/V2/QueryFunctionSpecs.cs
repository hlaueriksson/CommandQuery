#if NETCOREAPP2_0
using System;
using CommandQuery.AzureFunctions;
using CommandQuery.Exceptions;
using Machine.Fakes;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Specs.AzureFunctions.V2
{
    public class QueryFunctionSpecs
    {
        [Subject(typeof(QueryFunction))]
        public class when_handling_the_query : WithSubject<QueryFunction>
        {
            Establish context = () =>
            {
                Req = new DefaultHttpRequest(new DefaultHttpContext());
                Log = new FakeTraceWriter();
            };

            It should_handle_QueryValidationException = async () =>
            {
                var queryName = "FakeQuery";
                The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<string>())).Throw(new QueryValidationException("invalid"));

                var result = await Subject.Handle(queryName, Req, Log) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            };

            It should_handle_Exception = async () =>
            {
                var queryName = "FakeQuery";
                The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<string>())).Throw(new Exception("fail"));

                var result = await Subject.Handle(queryName, Req, Log) as ObjectResult;

                result.StatusCode.ShouldEqual(500);
                result.ShouldBeError("fail");
            };

            static DefaultHttpRequest Req;
            static FakeTraceWriter Log;
        }
    }
}
#endif