using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs.Host;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.Vs2.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                Req = GetHttpRequest("POST", content: "{ 'Id': 1 }");
                Log = new Mock<TraceWriter>(null).Object;
            }

            [Test]
            public async Task should_work()
            {
                var result = await Query.Run(Req, Log, "BarQuery") as OkObjectResult;
                var value = result.Value as Bar;

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Query.Run(Req, Log, "FailQuery") as BadRequestObjectResult;

                result.ShouldBeError("The query type 'FailQuery' could not be found");
            }

            DefaultHttpRequest Req;
            TraceWriter Log;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                Req = GetHttpRequest("GET", query: new Dictionary<string, string> { { "Id", "1" } });
                Log = new Mock<TraceWriter>(null).Object;
            }

            [Test]
            public async Task should_work()
            {
                var result = await Query.Run(Req, Log, "BarQuery") as OkObjectResult;
                var value = result.Value as Bar;

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Query.Run(Req, Log, "FailQuery") as BadRequestObjectResult;

                result.ShouldBeError("The query type 'FailQuery' could not be found");
            }

            DefaultHttpRequest Req;
            TraceWriter Log;
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