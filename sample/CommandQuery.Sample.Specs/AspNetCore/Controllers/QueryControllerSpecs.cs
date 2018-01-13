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

            It should_work = () =>
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/query/BarQuery", content).Result;

                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<Bar>().Result;

                result.Id.ShouldEqual(1);
                result.Value.ShouldNotBeEmpty();
            };

            It should_handle_errors = () =>
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/query/FailQuery", content).Result;

                response.IsSuccessStatusCode.ShouldBeFalse();

                var result = response.Content.ReadAsStringAsync().Result;

                result.ShouldEqual("The query type 'FailQuery' could not be found");
            };

            static TestServer Server;
            static HttpClient Client;
        }
    }
}