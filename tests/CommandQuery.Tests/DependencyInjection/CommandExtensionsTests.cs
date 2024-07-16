using System.Reflection;
using CommandQuery.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Tests.DependencyInjection
{
    public class CommandExtensionsTests
    {
        [LoFu, Test]
        public void when_GetCommandProcessor()
        {
            Assembly = typeof(FakeCommandHandler).Assembly;

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

            void should_add_commands_to_the_given_ServiceCollection()
            {
                Assembly.GetCommandProcessor(new ServiceCollection()).Should().NotBeNull();
                new[] { Assembly }.GetCommandProcessor(new ServiceCollection()).Should().NotBeNull();
                new ServiceCollection().GetCommandProcessor(Assembly).Should().NotBeNull();
            }
        }

        Assembly Assembly;
    }
}
