using System.Net.Http;
using System.Text;
using CommandQuery.Sample.Controllers;
using CommandQuery.Sample.Queries;
using Machine.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using It = Machine.Specifications.It;

namespace CommandQuery.Sample.Specs.Controllers
{
    public class QueryControllerSpecs
    {
        [Subject(typeof(QueryController))]
        public class when_using_the_real_API
        {
            Establish context = () =>
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            };

            It should_work = async () =>
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/query/BarQuery", content).Result; // NOTE: await does not work

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Bar>(responseString);

                result.Id.ShouldEqual(1);
                result.Value.ShouldNotBeEmpty();
            };

            It should_handle_errors = async () =>
            {
                var content = new StringContent("{ 'Id': 1 }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/query/FailQuery", content).Result; // NOTE: await does not work

                response.IsSuccessStatusCode.ShouldBeFalse();

                var responseString = await response.Content.ReadAsStringAsync();

                responseString.ShouldEqual("The query type 'FailQuery' could not be found");
            };

            static TestServer Server;
            static HttpClient Client;
        }
    }
}