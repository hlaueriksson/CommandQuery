using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using CommandQuery.Sample.AspNet.WebApi.Controllers;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.Sample.AspNet.WebApi.Tests
{
    public class QueryControllerTests
    {
        public class when_using_the_real_controller_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                var configuration = new HttpConfiguration();
                WebApiConfig.Register(configuration);

                var queryProcessor = configuration.DependencyResolver.GetService(typeof(IQueryProcessor)) as IQueryProcessor;

                Subject = new QueryController(queryProcessor, null)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = configuration
                };
            }

            [Test]
            public async Task should_work()
            {
                var json = JObject.Parse("{ 'Id': 1 }");
                var result = await (await Subject.HandlePost("BarQuery", json)).ExecuteAsync(CancellationToken.None);
                var value = await result.Content.ReadAsAsync<Bar>();

                result.EnsureSuccessStatusCode();
                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var json = JObject.Parse("{ 'Id': 1 }");
                var result = await Subject.HandlePost("FailQuery", json);

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            QueryController Subject;
        }

        public class when_using_the_real_controller_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                var configuration = new HttpConfiguration();
                WebApiConfig.Register(configuration);

                var queryProcessor = configuration.DependencyResolver.GetService(typeof(IQueryProcessor)) as IQueryProcessor;

                Subject = new QueryController(queryProcessor, null)
                {
                    Request = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://localhost/api/query/BarQuery?Id=1")
                    },
                    Configuration = configuration
                };
            }

            [Test]
            public async Task should_work()
            {
                var result = await (await Subject.HandleGet("BarQuery")).ExecuteAsync(CancellationToken.None);
                var value = await result.Content.ReadAsAsync<Bar>();

                result.EnsureSuccessStatusCode();
                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Subject.HandleGet("FailQuery");

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            QueryController Subject;
        }
    }
}