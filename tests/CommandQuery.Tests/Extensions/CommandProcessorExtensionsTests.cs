using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.Tests.Extensions
{
    public class CommandProcessorExtensionsTests
    {
        [LoFu, Test]
        public async Task when_processing_the_command()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            Subject = FakeCommandProcessor.Object;

            async Task should_create_the_command_from_a_string()
            {
                var expectedCommandType = typeof(FakeCommand);
                FakeCommandProcessor.Setup(x => x.GetCommandType(expectedCommandType.Name)).Returns(expectedCommandType);

                await Subject.ProcessAsync(expectedCommandType.Name, "{}");

                FakeCommandProcessor.Verify(x => x.ProcessAsync(It.IsAny<FakeCommand>()));
            }

            void should_throw_exception_if_the_command_type_is_not_found()
            {
                var commandName = "NotFoundCommand";
                var json = JObject.Parse("{}");

                Subject.Awaiting(async x => await x.ProcessAsync(commandName, json)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage("The command type 'NotFoundCommand' could not be found");
            }

            void should_throw_exception_if_the_json_is_invalid()
            {
                var commandName = "FakeCommand";

                Subject.Awaiting(async x => await x.ProcessAsync(commandName, (JObject)null)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage("The json could not be converted to an object");
            }
        }

        [LoFu, Test]
        public async Task when_processing_the_command_with_result()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            Subject = FakeCommandProcessor.Object;

            async Task should_create_the_command_from_a_string()
            {
                var expectedCommandType = typeof(FakeResultCommand);
                FakeCommandProcessor.Setup(x => x.GetCommandType(expectedCommandType.Name)).Returns(expectedCommandType);

                await Subject.ProcessWithResultAsync<FakeResult>(expectedCommandType.Name, "{}");

                FakeCommandProcessor.Verify(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>()));
            }

            void should_throw_exception_if_the_command_type_is_not_found()
            {
                var commandName = "NotFoundCommand";
                var json = JObject.Parse("{}");

                Subject.Awaiting(async x => await x.ProcessWithResultAsync<object>(commandName, json)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage("The command type 'NotFoundCommand' could not be found");
            }

            void should_throw_exception_if_the_json_is_invalid()
            {
                var commandName = "FakeResultCommand";

                Subject.Awaiting(async x => await x.ProcessWithResultAsync<object>(commandName, (JObject)null)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage("The json could not be converted to an object");
            }
        }

        [LoFu, Test]
        public async Task when_processing_the_command_with_or_without_result()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            Subject = FakeCommandProcessor.Object;

            async Task should_invoke_the_correct_command_handler_for_commands_without_result()
            {
                var expectedCommandType = typeof(FakeCommand);
                FakeCommandProcessor.Setup(x => x.GetCommandType(expectedCommandType.Name)).Returns(expectedCommandType);

                var result = await Subject.ProcessWithOrWithoutResultAsync(expectedCommandType.Name, "{}");

                result.Should().Be(CommandResult.None);
            }

            async Task should_invoke_the_correct_command_handler_for_commands_with_result()
            {
                var expectedResult = new FakeResult();
                var expectedCommandType = typeof(FakeResultCommand);
                FakeCommandProcessor.Setup(x => x.GetCommandType(expectedCommandType.Name)).Returns(expectedCommandType);
                FakeCommandProcessor.Setup(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>())).ReturnsAsync(expectedResult);

                var result = await Subject.ProcessWithOrWithoutResultAsync(expectedCommandType.Name, "{}");

                result.Value.Should().Be(expectedResult);
            }
        }

        Mock<ICommandProcessor> FakeCommandProcessor;
        ICommandProcessor Subject;
    }
}