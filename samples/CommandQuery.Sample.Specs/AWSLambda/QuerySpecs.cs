#if NETCOREAPP2_0
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.Sample.AWSLambda;
using CommandQuery.Sample.Queries;
using Machine.Specifications;

namespace CommandQuery.Sample.Specs.AWSLambda
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
                    Subject = new Query();
                    Request = GetRequest("POST", content: "{ 'Id': 1 }");
                    Context = new FakeLambdaContext();
                };

                It should_work = () =>
                {
                    var result = Subject.Handle(Request.QueryName("BarQuery"), Context).Result;
                    var value = result.As<Bar>();

                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var result = Subject.Handle(Request.QueryName("FailQuery"), Context).Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            public class method_Get
            {
                Establish context = () =>
                {
                    Subject = new Query();
                    Request = GetRequest("GET", query: new Dictionary<string, string> { { "Id", "1" } });
                    Context = new FakeLambdaContext();
                };

                It should_work = () =>
                {
                    var result = Subject.Handle(Request.QueryName("BarQuery"), Context).Result;
                    var value = result.As<Bar>();

                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var result = Subject.Handle(Request.QueryName("FailQuery"), Context).Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            static Query Subject;
            static APIGatewayProxyRequest Request;
            static ILambdaContext Context;

            static APIGatewayProxyRequest GetRequest(string method, string content = null, Dictionary<string, string> query = null)
            {
                var request = new APIGatewayProxyRequest
                {
                    HttpMethod = method,
                    Body = content,
                    QueryStringParameters = query
                };

                return request;
            }
        }
    }
}
#endif