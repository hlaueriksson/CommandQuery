using System.Text.Json;
using System.Web;
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
        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();
            new Startup().ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Subject = new Query(serviceProvider.GetRequiredService<IQueryFunction>());
            Context = new TestLambdaContext();
        }

        [Test]
        public async Task should_handle_query_via_Post()
        {
            var response = await Subject.Post(GetRequest("POST", new BarQuery { Id = 1 }), Context, "BarQuery");
            var value = response.Body<Bar>()!;
            value.Id.Should().Be(1);
        }

        [Test]
        public async Task should_handle_query_via_Get()
        {
            var response = await Subject.Get(GetRequest("GET", "?Id=1"), Context, "BarQuery");
            var value = response.Body<Bar>()!;
            value.Id.Should().Be(1);
        }

        static APIGatewayProxyRequest GetRequest(string method, object body) => new()
        {
            HttpMethod = method,
            Body = JsonSerializer.Serialize(body),
        };

        static APIGatewayProxyRequest GetRequest(string method, string query)
        {
            var collection = HttpUtility.ParseQueryString(query);
            var parameters = collection.AllKeys.ToDictionary(k => k!, k => collection.GetValues(k)!.ToList() as IList<string>);

            return new()
            {
                HttpMethod = method,
                MultiValueQueryStringParameters = parameters
            };
        }

        Query Subject = null!;
        ILambdaContext Context = null!;
    }
}
