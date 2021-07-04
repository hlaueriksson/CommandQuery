using System.Reflection;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests
{
    public class CommandTypeProviderTests
    {
        [LoFu, Test]
        public void when_getting_the_type()
        {
            Subject = new CommandTypeProvider(typeof(FakeCommand).GetTypeInfo().Assembly);

            void should_return_the_type_of_command_if_the_command_name_is_found() => Subject.GetCommandType("FakeCommand").Should().NotBeNull();

            void should_return_null_if_the_command_name_is_not_found() => Subject.GetCommandType("NotFoundCommand").Should().BeNull();
        }

        CommandTypeProvider Subject;
    }
}
