using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests
{
    public class CommandTypeProviderTests
    {
        [LoFu, Test]
        public void GetCommandType()
        {
            Subject = new CommandTypeProvider(typeof(FakeCommand).Assembly);

            void should_return_the_type_of_command_if_the_command_name_is_found() => Subject.GetCommandType("FakeCommand").Should().NotBeNull();

            void should_return_null_if_the_command_name_is_not_found() => Subject.GetCommandType("NotFoundCommand").Should().BeNull();
        }

        [LoFu, Test]
        public void GetCommandTypes()
        {
            Subject = new CommandTypeProvider(typeof(FakeCommand).Assembly);

            void should_return_the_types_of_commands_supported() => Subject.GetCommandTypes().Should().NotBeEmpty();
        }

        CommandTypeProvider Subject;
    }
}
