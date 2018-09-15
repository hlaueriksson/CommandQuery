using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.Sample.AspNet.WebApi.Controllers;
using Machine.Specifications;
using Newtonsoft.Json.Linq;

namespace CommandQuery.Sample.AspNet.WebApi.Specs.Controllers
{
    public class CommandControllerSpecs
    {
        [Subject(typeof(CommandController))]
        public class when_using_the_real_controller
        {
            private Establish context = () =>
            {
                var configuration = new HttpConfiguration();
                configuration.UseUnity();

                var commandProcessor = configuration.DependencyResolver.GetService(typeof(ICommandProcessor)) as ICommandProcessor;

                Subject = new CommandController(commandProcessor)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = configuration
                };
            };

            It should_work = () =>
            {
                var json = JObject.Parse("{ 'Value': 'Foo' }");
                var result = Subject.Handle("FooCommand", json).Result as OkResult;

                result.ShouldNotBeNull();
            };

            It should_handle_errors = () =>
            {
                var json = JObject.Parse("{ 'Value': 'Foo' }");
                var result = Subject.Handle("FailCommand", json).Result;

                result.ShouldBeError("The command type 'FailCommand' could not be found");
            };

            static CommandController Subject;
        }
    }
}