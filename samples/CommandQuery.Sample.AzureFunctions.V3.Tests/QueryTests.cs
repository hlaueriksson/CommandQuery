using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.V3.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                var mock = new Mock<IFunctionsHostBuilder>();
                mock.Setup(x => x.Services).Returns(serviceCollection);
                new Startup().Configure(mock.Object);
                ServiceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(ServiceProvider.GetService<IQueryFunction>());
                Req = GetHttpRequest("POST", content: "{ 'Id': 1 }");
                Log = new Mock<ILogger>().Object;
            }

            [Test]
            public void AssertConfigurationIsValid()
            {
                ServiceProvider.GetService<IQueryProcessor>().AssertConfigurationIsValid();
            }

            [Test]
            public async Task should_work()
            {
                var result = await Subject.Run(Req, Log, CancellationToken.None, "BarQuery") as ContentResult;
                var value = JsonConvert.DeserializeObject<Bar>(result.Content);

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Subject.Run(Req, Log, CancellationToken.None, "FailQuery") as ContentResult;

                result.ShouldBeError("The query type 'FailQuery' could not be found");
            }

            ServiceProvider ServiceProvider;
            Query Subject;
            DefaultHttpRequest Req;
            ILogger Log;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                var mock = new Mock<IFunctionsHostBuilder>();
                mock.Setup(x => x.Services).Returns(serviceCollection);
                new Startup().Configure(mock.Object);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(serviceProvider.GetService<IQueryFunction>());
                Req = GetHttpRequest("GET", query: new Dictionary<string, string> { { "Id", "1" } });
                Log = new Mock<ILogger>().Object;
            }

            [Test]
            public async Task should_work()
            {
                var result = await Subject.Run(Req, Log, CancellationToken.None, "BarQuery") as ContentResult;
                var value = JsonConvert.DeserializeObject<Bar>(result.Content);

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Subject.Run(Req, Log, CancellationToken.None, "FailQuery") as ContentResult;

                result.ShouldBeError("The query type 'FailQuery' could not be found");
            }

            Query Subject;
            DefaultHttpRequest Req;
            ILogger Log;
        }

        static DefaultHttpRequest GetHttpRequest(string method, string content = null, Dictionary<string, string> query = null)
        {
            var httpContext = new DefaultHttpContext();

            if (content != null)
            {
                httpContext.Features.Get<IHttpRequestFeature>().Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            }

            var request = new DefaultHttpRequest(httpContext) { Method = method };

            if (query != null)
            {
                request.QueryString = new QueryString(QueryHelpers.AddQueryString("", query));
            }

            return request;
        }
    }
}
