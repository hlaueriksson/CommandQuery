using System.Reflection;
using CommandQuery.DependencyInjection;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.DependencyInjection
{
    public class CommandExtensionsTests
    {
        [LoFu, Test]
        public void when_GetCommandProcessor()
        {
            Assembly = typeof(FakeCommandHandler).GetTypeInfo().Assembly;

            void should_add_commands_from_Assembly()
            {
                var result = Assembly.GetCommandProcessor();

                result.Should().NotBeNull();
                result.GetCommandTypes().Should().Contain(typeof(FakeCommand));
                result.GetCommandTypes().Should().Contain(typeof(FakeResultCommand));
            }

            void should_add_commands_from_Assemblies()
            {
                var result = new[] { Assembly }.GetCommandProcessor();

                result.Should().NotBeNull();
                result.GetCommandTypes().Should().Contain(typeof(FakeCommand));
                result.GetCommandTypes().Should().Contain(typeof(FakeResultCommand));
            }
        }

        Assembly Assembly;
    }
}