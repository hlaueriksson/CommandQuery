using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.SystemTextJson;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests.SystemTextJson
{
    public class CommandProcessorExtensionsTests
    {
        [LoFu, Test]
        public async Task when_processing_the_command_with_or_without_result()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            Subject = FakeCommandProcessor.Object;

            async Task should_invoke_the_correct_command_handler_for_commands_without_result()
            {
                var expectedCommandType = typeof(FakeCommand);
                FakeCommandProcessor.Setup(x => x.GetCommandType(expectedCommandType.Name)).Returns(expectedCommandType);
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Returns(Task.CompletedTask);

                var result = await Subject.ProcessAsync(expectedCommandType.Name, "{}");

                result.Should().Be(CommandResult.None);
                FakeCommandProcessor.Verify(x => x.ProcessAsync(It.IsAny<FakeCommand>()));
            }

            async Task should_invoke_the_correct_command_handler_for_commands_with_result()
            {
                var expectedResult = new FakeResult();
                var expectedCommandType = typeof(FakeResultCommand);
                FakeCommandProcessor.Setup(x => x.GetCommandType(expectedCommandType.Name)).Returns(expectedCommandType);
                FakeCommandProcessor.Setup(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>())).Returns(Task.FromResult(expectedResult));

                var result = await Subject.ProcessAsync(expectedCommandType.Name, "{}");

                result.Value.Should().Be(expectedResult);
                FakeCommandProcessor.Verify(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>()));
            }

            void should_throw_exception_if_the_ICommandProcessor_is_null()
            {
                Subject.Awaiting(x => ((ICommandProcessor)null).ProcessAsync("", "{}")).Should()
                    .Throw<ArgumentNullException>();
            }

            void should_throw_exception_if_the_command_type_is_not_found()
            {
                var commandName = "NotFoundCommand";

                Subject.Awaiting(x => x.ProcessAsync(commandName, "{}")).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage("The command type 'NotFoundCommand' could not be found");
            }

            void should_throw_exception_if_the_json_is_invalid()
            {
                var commandName = "FakeCommand";

                Subject.Awaiting(x => x.ProcessAsync(commandName, "<>")).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage("The json string could not be deserialized to an object");
            }

            void should_throw_exception_if_the_json_is_null()
            {
                var commandName = "FakeCommand";

                Subject.Awaiting(x => x.ProcessAsync(commandName, null)).Should()
                    .Throw<ArgumentNullException>();
            }
        }

        Mock<ICommandProcessor> FakeCommandProcessor;
        ICommandProcessor Subject;
    }
}
