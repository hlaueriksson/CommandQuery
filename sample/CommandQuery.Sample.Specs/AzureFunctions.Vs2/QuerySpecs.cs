using System.IO;
using CommandQuery.Sample.AzureFunctions.Vs2;
using CommandQuery.Sample.Queries;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs2
{
    public class QuerySpecs
    {
        [Subject(typeof(Query))]
        public class when_using_the_real_function
        {
            It should_work = () =>
            {
                var req = GetHttpRequest("{ 'Id': 1 }");
                var log = new FakeTraceWriter();

                var result = Query.Run(req, log, "BarQuery").Result as OkObjectResult;
                var value = result.Value as Bar;

                value.Id.ShouldEqual(1);
                value.Value.ShouldNotBeEmpty();
            };

            It should_handle_errors = () =>
            {
                var req = GetHttpRequest("{ 'Id': 1 }");
                var log = new FakeTraceWriter();

                var result = Query.Run(req, log, "FailQuery").Result as BadRequestObjectResult;

                result.Value.ShouldEqual("The query type 'FailQuery' could not be found");
            };

            static DefaultHttpRequest GetHttpRequest(string content)
            {
                var httpContext = new DefaultHttpContext();
                httpContext.Features.Get<IHttpRequestFeature>().Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

                return new DefaultHttpRequest(httpContext);
            }
        }
    }
}