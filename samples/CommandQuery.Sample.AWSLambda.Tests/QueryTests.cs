using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using CommandQuery.AWSLambda;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.Sample.AWSLambda.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(serviceProvider.GetRequiredService<IQueryFunction>());
                Request = GetRequest("POST", content: "{ \"Id\": 1 }");
                Context = new TestLambdaContext();
            }

            [Test]
            public async Task should_work()
            {
                var result = await Subject.Post(Request, Context, "BarQuery");
                var value = result.As<Bar>()!;

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Subject.Post(Request, Context, "FailQuery");

                result.ShouldBeError("The query type 'FailQuery' could not be found");
            }

            Query Subject = null!;
            APIGatewayProxyRequest Request = null!;
            ILambdaContext Context = null!;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                var serviceCollection = new ServiceCollection();
                new Startup().ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                Subject = new Query(serviceProvider.GetRequiredService<IQueryFunction>());
                Request = GetRequest("GET", query: new Dictionary<string, IList<string>> { { "Id", new List<string> { "1" } } });
                Context = new TestLambdaContext();
            }

            [Test]
            public async Task should_work()
            {
                var result = await Subject.Get(Request, Context, "BarQuery");
                var value = result.As<Bar>()!;

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Subject.Get(Request, Context, "FailQuery");

                result.ShouldBeError("The query type 'FailQuery' could not be found");
            }

            Query Subject = null!;
            APIGatewayProxyRequest Request = null!;
            ILambdaContext Context = null!;
        }

        static APIGatewayProxyRequest GetRequest(string method, string? content = null, Dictionary<string, IList<string>>? query = null)
        {
            var request = new APIGatewayProxyRequest
            {
                HttpMethod = method,
                Body = content,
                MultiValueQueryStringParameters = query
            };

            return request;
        }
    }
}
