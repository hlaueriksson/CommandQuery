using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using NUnit.Framework;

namespace CommandQuery.Sample.AzureFunctions.Vs1.Tests
{
    public class QueryTests
    {
        public class when_using_the_real_function_via_Post
        {
            [SetUp]
            public void SetUp()
            {
                Req = GetHttpRequest(HttpMethod.Post, content: "{ 'Id': 1 }");
                Log = new FakeTraceWriter();
            }

            [Test]
            public async Task should_work()
            {
                var result = await Query.Run(Req, Log, "BarQuery");
                var value = await result.Content.ReadAsAsync<Bar>();

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Query.Run(Req, Log, "FailQuery");

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            HttpRequestMessage Req;
            FakeTraceWriter Log;
        }

        public class when_using_the_real_function_via_Get
        {
            [SetUp]
            public void SetUp()
            {
                Req = GetHttpRequest(HttpMethod.Get, query: new Dictionary<string, string> { { "Id", "1" } });
                Log = new FakeTraceWriter();
            }

            [Test]
            public async Task should_work()
            {
                var result = await Query.Run(Req, Log, "BarQuery");
                var value = await result.Content.ReadAsAsync<Bar>();

                value.Id.Should().Be(1);
                value.Value.Should().NotBeEmpty();
            }

            [Test]
            public async Task should_handle_errors()
            {
                var result = await Query.Run(Req, Log, "FailQuery");

                await result.ShouldBeErrorAsync("The query type 'FailQuery' could not be found");
            }

            HttpRequestMessage Req;
            FakeTraceWriter Log;
        }

        static HttpRequestMessage GetHttpRequest(HttpMethod method, string content = null, Dictionary<string, string> query = null)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage();
            request.SetConfiguration(config);
            request.Method = method;

            if (content != null)
            {
                request.Content = new StringContent(content);
            }

            if (query != null)
            {
                request.RequestUri = new Uri(QueryHelpers.AddQueryString("https://example.com", query));
            }

            return request;
        }
    }
}