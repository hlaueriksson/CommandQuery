using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.V6.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
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

                Subject = new Query(serviceProvider.GetService<IQueryFunction>()!, serviceProvider.GetService<ILoggerFactory>()!);
            }

            [Test]
            public async Task should_work()
            {
                var req = GetHttpRequestData(ExecutionContext, "POST", content: "{ \"Id\": 1 }");

                var result = await Subject.Run(req, ExecutionContext, "BarQuery");
                var value = await result.AsAsync<Bar>();

                value!.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var req = GetHttpRequestData(ExecutionContext, "POST", content: "{ \"Id\": 1 }");

                var result = await Subject.Run(req, ExecutionContext, "FailQuery");

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            FunctionContext ExecutionContext = null!;
            Query Subject = null!;
        }

        public class when_using_the_real_function_via_Get
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

                Subject = new Query(serviceProvider.GetService<IQueryFunction>()!, serviceProvider.GetService<ILoggerFactory>()!);
            }

            [Test]
            public async Task should_work()
            {
                var req = GetHttpRequestData(ExecutionContext, "GET", url: "http://localhost?Id=1");

                var result = await Subject.Run(req, ExecutionContext, "BarQuery");
                var value = await result.AsAsync<Bar>();

                value!.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var req = GetHttpRequestData(ExecutionContext, "GET", url: "http://localhost?Id=1");

                var result = await Subject.Run(req, ExecutionContext, "FailQuery");

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            FunctionContext ExecutionContext = null!;
            Query Subject = null!;
        }

        static HttpRequestData GetHttpRequestData(FunctionContext executionContext, string method, string? content = null, string? url = null)
        {
            var request = new Mock<HttpRequestData>(executionContext);
            request.Setup(r => r.Method).Returns(method);

            if (content != null)
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                request.Setup(r => r.Body).Returns(stream);
            }

            if (url != null)
            {
                request.Setup(r => r.Url).Returns(new Uri(url));
            }

            request.Setup(r => r.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(executionContext);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode);
                response.SetupProperty(r => r.Body, new MemoryStream());
                return response.Object;
            });

            return request.Object;
        }
    }
}
