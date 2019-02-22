﻿using System.Reflection;
using CommandQuery.DependencyInjection;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.DependencyInjection
{
    public class QueryExtensionsTests
    {
        [LoFu, Test]
        public void when_GetQueryProcessor()
        {
            Assembly = typeof(FakeQueryHandler).GetTypeInfo().Assembly;

            void should_add_queries_from_Assembly()
            {
                var result = Assembly.GetQueryProcessor();

                result.Should().NotBeNull();
                result.GetQueries().Should().Contain(typeof(FakeQuery));
            }

            void should_add_queries_from_Assemblies()
            {
                var result = new[] { Assembly }.GetQueryProcessor();

                result.Should().NotBeNull();
                result.GetQueries().Should().Contain(typeof(FakeQuery));
            }
        }

        Assembly Assembly;
    }
}