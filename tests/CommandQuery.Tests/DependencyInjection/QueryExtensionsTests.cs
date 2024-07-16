using System.Reflection;
using CommandQuery.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Tests.DependencyInjection
{
    public class QueryExtensionsTests
    {
        [LoFu, Test]
        public void when_GetQueryProcessor()
        {
            Assembly = typeof(FakeQueryHandler).Assembly;

            void should_add_queries_from_Assembly()
            {
                var result = Assembly.GetQueryProcessor();

                result.Should().NotBeNull();
                result.GetQueryTypes().Should().Contain(typeof(FakeQuery));
            }

            void should_add_queries_from_Assemblies()
            {
                var result = new[] { Assembly }.GetQueryProcessor();

                result.Should().NotBeNull();
                result.GetQueryTypes().Should().Contain(typeof(FakeQuery));
            }

            void should_add_queries_to_the_given_ServiceCollection()
            {
                Assembly.GetQueryProcessor(new ServiceCollection()).Should().NotBeNull();
                new[] { Assembly }.GetQueryProcessor(new ServiceCollection()).Should().NotBeNull();
                new ServiceCollection().GetQueryProcessor(Assembly).Should().NotBeNull();
            }
        }

        Assembly Assembly;
    }
}
