using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.V3.Tests
{
    public class CommandTests
    {
        public class when_using_the_real_function
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                var mock = new Mock<IFunctionsHostBuilder>();
                mock.Setup(x => x.Services).Returns(serviceCollection);
                new Startup().Configure(mock.Object);
                ServiceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Command(ServiceProvider.GetService<ICommandFunction>());
            }

            [Test]
            public void AssertConfigurationIsValid()
            {
                ServiceProvider.GetService<ICommandProcessor>().AssertConfigurationIsValid();
            }

            [Test]
            public async Task should_work()
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new Mock<ILogger>();

                var result = await Subject.Run(req, log.Object, CancellationToken.None, "FooCommand") as OkResult;

                result.Should().NotBeNull();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var req = GetHttpRequest("{ 'Value': 'Foo' }");
                var log = new Mock<ILogger>();

                var result = await Subject.Run(req, log.Object, CancellationToken.None, "FailCommand") as ContentResult;

                result.ShouldBeError("The command type 'FailCommand' could not be found");
            }

            DefaultHttpRequest GetHttpRequest(string content)
            {
                var httpContext = new DefaultHttpContext();
                httpContext.Features.Get<IHttpRequestFeature>().Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

                return new DefaultHttpRequest(httpContext);
            }

            ServiceProvider ServiceProvider;
            Command Subject;
        }
    }
}
