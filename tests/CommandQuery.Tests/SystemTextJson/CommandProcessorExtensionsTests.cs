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
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

                var result = await Subject.ProcessAsync(expectedCommandType.Name, "{}");

                result.Should().Be(CommandResult.None);
                FakeCommandProcessor.Verify(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>()));
            }

            async Task should_invoke_the_correct_command_handler_for_commands_with_result()
            {
                var expectedResult = new FakeResult();
                var expectedCommandType = typeof(FakeResultCommand);
                FakeCommandProcessor.Setup(x => x.GetCommandType(expectedCommandType.Name)).Returns(expectedCommandType);
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expectedResult));

                var result = await Subject.ProcessAsync(expectedCommandType.Name, "{}");

                result.Value.Should().Be(expectedResult);
                FakeCommandProcessor.Verify(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>()));
            }

            async Task should_throw_exception_if_the_ICommandProcessor_is_null()
            {
                Func<Task> act = () => ((ICommandProcessor)null).ProcessAsync("", "{}");
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_throw_exception_if_the_command_type_is_not_found()
            {
                var commandName = "NotFoundCommand";

                Func<Task> act = () => Subject.ProcessAsync(commandName, "{}");
                await act.Should().ThrowAsync<CommandProcessorException>()
                    .WithMessage("The command type 'NotFoundCommand' could not be found");
            }

            async Task should_throw_exception_if_the_json_is_invalid()
            {
                var commandName = "FakeCommand";

                Func<Task> act = () => Subject.ProcessAsync(commandName, "<>");
                await act.Should().ThrowAsync<CommandProcessorException>()
                    .WithMessage("The json string could not be deserialized to an object");
            }

            async Task should_throw_exception_if_the_json_is_null()
            {
                var commandName = "FakeCommand";

                Func<Task> act = () => Subject.ProcessAsync(commandName, null);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }
        }

        Mock<ICommandProcessor> FakeCommandProcessor;
        ICommandProcessor Subject;
    }
}
