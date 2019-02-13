using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.Sample.AspNet.WebApi.Controllers;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNet.WebApi.Tests
{
    public class CommandControllerTests
    {
        public class when_using_the_real_controller
        {
            [SetUp]
            public void SetUp()
            {
                var configuration = new HttpConfiguration();
                WebApiConfig.Register(configuration);

                var commandProcessor = configuration.DependencyResolver.GetService(typeof(ICommandProcessor)) as ICommandProcessor;

                Subject = new CommandController(commandProcessor, null)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = configuration
                };
            }

            [Test]
            public async Task should_work()
            {
                var json = JObject.Parse("{ 'Value': 'Foo' }");
                var result = await Subject.Handle("FooCommand", json) as OkResult;

                result.Should().NotBeNull();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var json = JObject.Parse("{ 'Value': 'Foo' }");
                var result = await Subject.Handle("FailCommand", json);

                await result.ShouldBeErrorAsync("The command type 'FailCommand' could not be found");
            }

            CommandController Subject;
        }
    }
}