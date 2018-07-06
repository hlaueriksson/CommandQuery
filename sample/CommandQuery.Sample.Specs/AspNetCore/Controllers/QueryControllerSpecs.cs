#if NETCOREAPP2_0
using System.Net.Http;
using System.Text;
using CommandQuery.Sample.Queries;
using CommandQuery.Sample.AspNetCore;
using CommandQuery.Sample.AspNetCore.Controllers;
using Machine.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace CommandQuery.Sample.Specs.AspNetCore.Controllers
{
    public class QueryControllerSpecs
    {
        [Subject(typeof(QueryController))]
        public class when_using_the_real_controller
        {
            Establish context = () =>
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            };

            public class method_Post
            {
                It should_work = () =>
                {
                    var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                    var result = Client.PostAsync("/api/query/BarQuery", content).Result;
                    var value = result.Content.ReadAsAsync<Bar>().Result;

                    result.EnsureSuccessStatusCode();
                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                    var result = Client.PostAsync("/api/query/FailQuery", content).Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            public class method_Get
            {
                It should_work = () =>
                {
                    var result = Client.GetAsync("/api/query/BarQuery?Id=1").Result;
                    var value = result.Content.ReadAsAsync<Bar>().Result;

                    result.EnsureSuccessStatusCode();
                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var result = Client.GetAsync("/api/query/FailQuery?Id=1").Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            static TestServer Server;
            static HttpClient Client;
        }
    }
}
#endif