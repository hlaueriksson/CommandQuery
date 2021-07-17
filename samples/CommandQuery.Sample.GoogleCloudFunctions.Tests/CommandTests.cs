using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.GoogleCloudFunctions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.Sample.GoogleCloudFunctions.Tests
{
    public class CommandTests
    {
        public class when_using_the_real_function
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(null, serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Command(null, serviceProvider.GetService<ICommandFunction>());
            }

            [Test]
            public async Task should_work()
            {
                var context = GetHttpContext("FooCommand", "{ \"Value\": \"Foo\" }");

                await Subject.HandleAsync(context);

                context.Response.StatusCode.Should().Be(200);
            }

            [Test]
            public async Task should_handle_errors()
            {
                var context = GetHttpContext("FailCommand", "{ \"Value\": \"Foo\" }");

                await Subject.HandleAsync(context);

                await context.Response.ShouldBeErrorAsync("The command type 'FailCommand' could not be found");
            }

            HttpContext GetHttpContext(string commandName, string content)
            {
                var context = new DefaultHttpContext();
                context.Request.Path = new PathString("/api/command/" + commandName);
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(content));
                context.Response.Body = new MemoryStream();

                return context;
            }

            Command Subject;
        }
    }
}
