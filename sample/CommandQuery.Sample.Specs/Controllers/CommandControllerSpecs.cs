using System.Net.Http;
using System.Text;
using CommandQuery.Sample.Controllers;
using Machine.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using It = Machine.Specifications.It;

namespace CommandQuery.Sample.Specs.Controllers
{
    public class CommandControllerSpecs
    {
        [Subject(typeof(CommandController))]
        public class when_using_the_real_API
        {
            Establish context = () =>
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            };

            It should_work = async () =>
            {
                var content = new StringContent("{ 'Value': 'Foo' }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/command/FooCommand", content).Result; // NOTE: await does not work

                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                responseString.ShouldBeEmpty();
            };

            It should_handle_errors = async () =>
            {
                var content = new StringContent("{ 'Value': 'Foo' }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/command/FailCommand", content).Result; // NOTE: await does not work

                response.IsSuccessStatusCode.ShouldBeFalse();

                var responseString = await response.Content.ReadAsStringAsync();

                responseString.ShouldEqual("The command type 'FailCommand' could not be found");
            };

            static TestServer Server;
            static HttpClient Client;
        }
    }
}