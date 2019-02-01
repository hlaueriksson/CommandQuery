#if NETCOREAPP2_0
using System.Collections.Generic;
using System.IO;
using CommandQuery.Sample.AzureFunctions.Vs2;
using CommandQuery.Sample.Queries;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs.Host;
using Moq;
using It = Machine.Specifications.It;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs2
{
    public class QuerySpecs
    {
        [Subject(typeof(Query))]
        public class when_using_the_real_function
        {
            public class method_Post
            {
                Establish context = () =>
                {
                    Req = GetHttpRequest("POST", content: "{ 'Id': 1 }");
                    Log = new Mock<TraceWriter>().Object;
                };

                It should_work = () =>
                {
                    var result = Query.Run(Req, Log, "BarQuery").Result as OkObjectResult;
                    var value = result.Value as Bar;

                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var result = Query.Run(Req, Log, "FailQuery").Result as BadRequestObjectResult;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            public class method_Get
            {
                Establish context = () =>
                {
                    Req = GetHttpRequest("GET", query: new Dictionary<string, string> { { "Id", "1" } });
                    Log = new Mock<TraceWriter>().Object;
                };

                It should_work = () =>
                {
                    var result = Query.Run(Req, Log, "BarQuery").Result as OkObjectResult;
                    var value = result.Value as Bar;

                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var result = Query.Run(Req, Log, "FailQuery").Result as BadRequestObjectResult;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            static DefaultHttpRequest Req;
            static TraceWriter Log;

            static DefaultHttpRequest GetHttpRequest(string method, string content = null, Dictionary<string, string> query = null)
            {
                var httpContext = new DefaultHttpContext();

                if (content != null)
                {
                    httpContext.Features.Get<IHttpRequestFeature>().Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
                }

                var request = new DefaultHttpRequest(httpContext) { Method = method };

                if (query != null)
                {
                    request.QueryString = new QueryString(QueryHelpers.AddQueryString("", query));
                }

                return request;
            }
        }
    }
}
#endif