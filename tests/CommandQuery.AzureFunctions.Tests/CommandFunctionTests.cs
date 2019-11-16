using System;
using System.Threading.Tasks;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            Use<Mock<ICommandProcessor>>();

            async Task should_invoke_the_command_processor()
            {
                var commandName = "FakeCommand";
                var content = "{}";

                await Subject.Handle(commandName, content);

                The<Mock<ICommandProcessor>>().Verify(x => x.ProcessAsync(commandName, content));
            }

            async Task base_method_should_not_handle_Exception()
            {
                var commandName = "FakeCommand";
                var content = "{}";

                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, content)).Throws(new Exception("fail"));

                Subject.Awaiting(async x => await x.Handle(commandName, content)).Should()
                    .Throw<Exception>()
                    .WithMessage("fail");
            }
        }
    }
}