﻿using System;
using System.Threading.Tasks;
using CommandQuery.Client;
using CommandQuery.Sample.Contracts.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Tests.Client
{
    [Explicit("Integration tests")]
    public class QueryClientTests
    {
        [SetUp]
        public void SetUp()
        {
            Subject = new QueryClient("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/query/");
        }

        [Test]
        public void when_Post()
        {
            var result = Subject.Post(new BarQuery { Id = 1 });
            result.Should().NotBeNull();
        }

        [Test]
        public async Task when_PostAsync()
        {
            var result = await Subject.PostAsync(new BarQuery { Id = 1 });
            result.Should().NotBeNull();
        }

        [Test]
        public void when_Get()
        {
            var result = Subject.Get(new QuxQuery { Ids = new[] { Guid.NewGuid(), Guid.NewGuid() } });
            result.Should().NotBeNull();
        }

        [Test]
        public async Task when_GetAsync()
        {
            var result = await Subject.GetAsync(new QuxQuery { Ids = new[] { Guid.NewGuid(), Guid.NewGuid() } });
            result.Should().NotBeNull();
        }
        
        [Test]
        public void when_configuring_the_client()
        {
            var client = new QueryClient("http://example.com", x => x.BaseAddress = new Uri("https://commandquery-sample-azurefunctions-vs2.azurewebsites.net/api/query/"));
            client.Post(new BarQuery {Id = 1});
        }

        QueryClient Subject;
    }
}