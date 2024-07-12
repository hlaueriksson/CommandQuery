using System.Text;
using System.Text.Json;
using System.Web;
using CommandQuery.AzureFunctions;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.Tests
{
    public class QueryTests
    {
        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();
            Program.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Subject = new Query(serviceProvider.GetRequiredService<IQueryFunction>());

            var context = new Mock<FunctionContext>();
            context.SetupProperty(c => c.InstanceServices, serviceProvider);
            Context = context.Object;
        }

        [Test]
        public async Task should_handle_query_via_Post()
        {
            var result = await Subject.Run(GetRequest("POST", new BarQuery { Id = 1 }), Context, "BarQuery");
            var value = result.Value<Bar>()!;
            value.Id.Should().Be(1);
        }

        [Test]
        public async Task should_handle_query_via_Get()
        {
            var result = await Subject.Run(GetRequest("GET", "?Id=1"), Context, "BarQuery");
            var value = result.Value<Bar>()!;
            value.Id.Should().Be(1);
        }

        static HttpRequest GetRequest(string method, object body)
        {
            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Method).Returns(method);
            request.Setup(r => r.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(body))));
            return request.Object;
        }

        static HttpRequest GetRequest(string method, string query)
        {
            var collection = HttpUtility.ParseQueryString(query);
            var store = collection.AllKeys.ToDictionary(k => k!, k => new StringValues(collection.GetValues(k)));

            var request = new Mock<HttpRequest>();
            request.Setup(r => r.Method).Returns(method);
            request.Setup(r => r.Query).Returns(new QueryCollection(store));
            return request.Object;
        }

        Query Subject = null!;
        FunctionContext Context = null!;
    }
}
