using System.Net.Http;
using System.Text;
using CommandQuery.Sample.AspNetCore;
using CommandQuery.Sample.AspNetCore.Controllers;
using Machine.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace CommandQuery.Sample.Specs.AspNetCore.Controllers
{
    public class CommandControllerSpecs
    {
        [Subject(typeof(CommandController))]
        public class when_using_the_real_controller
        {
            Establish context = () =>
            {
                Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                Client = Server.CreateClient();
            };

            It should_work = () =>
            {
                var content = new StringContent("{ 'Value': 'Foo' }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/command/FooCommand", content).Result;

                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsStringAsync().Result;

                result.ShouldBeEmpty();
            };

            It should_handle_errors = () =>
            {
                var content = new StringContent("{ 'Value': 'Foo' }", Encoding.UTF8, "application/json");
                var response = Client.PostAsync("/api/command/FailCommand", content).Result;

                response.IsSuccessStatusCode.ShouldBeFalse();

                var result = response.Content.ReadAsStringAsync().Result;

                result.ShouldEqual("The command type 'FailCommand' could not be found");
            };

            static TestServer Server;
            static HttpClient Client;
        }
    }
}