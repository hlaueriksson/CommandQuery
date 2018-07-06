#if NET461
using System;
using System.Collections.Generic;
using CommandQuery.Sample.AzureFunctions.Vs1;
using CommandQuery.Sample.Queries;
using Machine.Specifications;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs1
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
                    Req = GetHttpRequest(HttpMethod.Post, content: "{ 'Id': 1 }");
                    Log = new FakeTraceWriter();
                };

                It should_work = () =>
                {
                    var result = Query.Run(Req, Log, "BarQuery").Result;
                    var value = result.Content.ReadAsAsync<Bar>().Result;

                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var result = Query.Run(Req, Log, "FailQuery").Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            public class method_Get
            {
                Establish context = () =>
                {
                    Req = GetHttpRequest(HttpMethod.Get, query: new Dictionary<string, string> { { "Id", "1" } });
                    Log = new FakeTraceWriter();
                };

                It should_work = () =>
                {
                    var result = Query.Run(Req, Log, "BarQuery").Result;
                    var value = result.Content.ReadAsAsync<Bar>().Result;

                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var result = Query.Run(Req, Log, "FailQuery").Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            static HttpRequestMessage Req;
            static FakeTraceWriter Log;

            static HttpRequestMessage GetHttpRequest(HttpMethod method, string content = null, Dictionary<string, string> query = null)
            {
                var config = new HttpConfiguration();
                var request = new HttpRequestMessage();
                request.SetConfiguration(config);
                request.Method = method;

                if (content != null)
                {
                    request.Content = new StringContent(content);
                }

                if (query != null)
                {
                    request.RequestUri = new Uri(QueryHelpers.AddQueryString("https://example.com", query));
                }

                return request;
            }
        }
    }
}
#endif