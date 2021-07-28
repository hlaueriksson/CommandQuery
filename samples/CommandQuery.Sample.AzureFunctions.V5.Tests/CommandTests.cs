using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.V5.Tests
{
    public class CommandTests
    {
        public class when_using_the_real_function
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
                Program.ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                var context = new Mock<FunctionContext>();
                context.SetupProperty(c => c.InstanceServices, serviceProvider);
                ExecutionContext = context.Object;

                Subject = new Command(serviceProvider.GetService<ICommandFunction>());
            }

            [Test]
            public async Task should_work()
            {
                var req = GetHttpRequestData("{ \"Value\": \"Foo\" }");

                var result = await Subject.Run(req, ExecutionContext, "FooCommand");

                result.Should().NotBeNull();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var req = GetHttpRequestData("{ \"Value\": \"Foo\" }");

                var result = await Subject.Run(req, ExecutionContext, "FailCommand");

                await result.ShouldBeErrorAsync("The command type 'FailCommand' could not be found");
            }

            HttpRequestData GetHttpRequestData(string content)
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

                var request = new Mock<HttpRequestData>(ExecutionContext);
                request.Setup(r => r.Body).Returns(stream);
                request.Setup(r => r.CreateResponse()).Returns(() =>
                {
                    var response = new Mock<HttpResponseData>(ExecutionContext);
                    response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                    response.SetupProperty(r => r.StatusCode);
                    response.SetupProperty(r => r.Body, new MemoryStream());
                    return response.Object;
                });

                return request.Object;
            }

            FunctionContext ExecutionContext;
            Command Subject;
        }
    }
}
