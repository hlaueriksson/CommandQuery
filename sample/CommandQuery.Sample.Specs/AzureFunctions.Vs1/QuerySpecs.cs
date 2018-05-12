#if NET461
using CommandQuery.Sample.AzureFunctions.Vs1;
using CommandQuery.Sample.Queries;
using Machine.Specifications;
using System.Net.Http;
using System.Web.Http;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs1
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

                var result = Query.Run(req, log, "BarQuery").Result;
                var value = result.Content.ReadAsAsync<Bar>().Result;

                value.Id.ShouldEqual(1);
                value.Value.ShouldNotBeEmpty();
            };

            It should_handle_errors = () =>
            {
                var req = GetHttpRequest("{ 'Id': 1 }");
                var log = new FakeTraceWriter();

                var result = Query.Run(req, log, "FailQuery").Result;

                result.ShouldBeError("The query type 'FailQuery' could not be found");
            };

            static HttpRequestMessage GetHttpRequest(string content)
            {
                var config = new HttpConfiguration();
                var request = new HttpRequestMessage();
                request.SetConfiguration(config);
                request.Content = new StringContent(content);

                return request;
            }
        }
    }
}
#endif