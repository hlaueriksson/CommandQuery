using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.GoogleCloudFunctions;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.Sample.GoogleCloudFunctions.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(null!, serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(null!, serviceProvider.GetService<IQueryFunction>());
            }

            [Test]
            public async Task should_work()
            {
                var context = GetHttpContext("BarQuery", "POST", content: "{ \"Id\": 1 }");

                await Subject.HandleAsync(context);
                var value = await context.Response.AsAsync<Bar>();

                value!.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var context = GetHttpContext("FailQuery", "POST", content: "{ \"Id\": 1 }");

                await Subject.HandleAsync(context);

                await context.Response.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            Query Subject = null!;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(null!, serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(null!, serviceProvider.GetService<IQueryFunction>());
            }

            [Test]
            public async Task should_work()
            {
                var context = GetHttpContext("BarQuery", "GET", query: new Dictionary<string, string> { { "Id", "1" } });

                await Subject.HandleAsync(context);
                var value = await context.Response.AsAsync<Bar>();

                value!.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var context = GetHttpContext("FailQuery", "GET", query: new Dictionary<string, string> { { "Id", "1" } });

                await Subject.HandleAsync(context);

                await context.Response.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            Query Subject = null!;
        }

        static HttpContext GetHttpContext(string queryName, string method, string? content = null, Dictionary<string, string>? query = null)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = new PathString("/api/query/" + queryName);
            context.Request.Method = method;

            if (content != null)
            {
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(content));
            }

            if (query != null)
            {
                context.Request.QueryString = new QueryString(QueryHelpers.AddQueryString("", query));
            }

            context.Response.Body = new MemoryStream();

            return context;
        }
    }
}
