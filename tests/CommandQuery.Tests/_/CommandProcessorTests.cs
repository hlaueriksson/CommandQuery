using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.Tests._
{
    public class CommandProcessorTests
    {
        [LoFu, Test]
        public async Task when_processing_the_command()
        {
            FakeCommandTypeCollection = new Mock<ICommandTypeCollection>();
            FakeServiceProvider = new Mock<IServiceProvider>();
            Subject = new CommandProcessor(FakeCommandTypeCollection.Object, FakeServiceProvider.Object);

            async Task should_invoke_the_correct_command_handler()
            {
                FakeCommand expectedCommand = null;
                var fakeCommandHandler = new FakeCommandHandler(x => expectedCommand = x);
                FakeServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>))).Returns(fakeCommandHandler);

                var command = new FakeCommand();
                await Subject.ProcessAsync(command);

                command.Should().Be(expectedCommand);
            }

            async Task should_create_the_command_from_a_string()
            {
                var expectedCommandType = typeof(FakeCommand);
                var fakeCommandHandler = new Mock<ICommandHandler<FakeCommand>>();
                FakeCommandTypeCollection.Setup(x => x.GetType(expectedCommandType.Name)).Returns(expectedCommandType);
                FakeServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>))).Returns(fakeCommandHandler.Object);

                await Subject.ProcessAsync(expectedCommandType.Name, "{}");

                fakeCommandHandler.Verify(x => x.HandleAsync(It.IsAny<FakeCommand>()));
            }

            void should_throw_exception_if_the_command_handler_is_not_found()
            {
                var command = new Mock<ICommand>().Object;

                Subject.Awaiting(async x => await x.ProcessAsync(command)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage($"The command handler for '{command}' could not be found");
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
                FakeCommandTypeCollection.Setup(x => x.GetType(commandName)).Returns(typeof(FakeCommand));

                Subject.Awaiting(async x => await x.ProcessAsync(commandName, (JObject)null)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage("The json could not be converted to an object");
            }
        }

        [LoFu, Test]
        public async Task when_processing_the_command_with_result()
        {
            FakeCommandTypeCollection = new Mock<ICommandTypeCollection>();
            FakeServiceProvider = new Mock<IServiceProvider>();
            Subject = new CommandProcessor(FakeCommandTypeCollection.Object, FakeServiceProvider.Object);

            async Task should_invoke_the_correct_command_handler_and_return_a_result()
            {
                FakeResultCommand expectedCommand = null;
                var expectedResult = new FakeResult();
                var fakeCommandHandler = new FakeResultCommandHandler(x => { expectedCommand = x; return expectedResult; });
                FakeServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<FakeResultCommand, FakeResult>))).Returns(fakeCommandHandler);

                var command = new FakeResultCommand();
                var result = await Subject.ProcessWithResultAsync(command);

                command.Should().Be(expectedCommand);
                result.Should().Be(expectedResult);
            }

            async Task should_create_the_command_from_a_string()
            {
                var expectedCommandType = typeof(FakeResultCommand);
                var fakeCommandHandler = new Mock<ICommandHandler<FakeResultCommand, FakeResult>>();
                FakeCommandTypeCollection.Setup(x => x.GetType(expectedCommandType.Name)).Returns(expectedCommandType);
                FakeServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<FakeResultCommand, FakeResult>))).Returns(fakeCommandHandler.Object);

                await Subject.ProcessWithResultAsync<FakeResult>(expectedCommandType.Name, "{}");

                fakeCommandHandler.Verify(x => x.HandleAsync(It.IsAny<FakeResultCommand>()));
            }

            void should_throw_exception_if_the_command_handler_is_not_found()
            {
                var command = new Mock<ICommand<object>>().Object;

                Subject.Awaiting(async x => await x.ProcessWithResultAsync(command)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage($"The command handler for '{command}' could not be found");
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
                var commandName = "FakeCommand";
                FakeCommandTypeCollection.Setup(x => x.GetType(commandName)).Returns(typeof(FakeCommand));

                Subject.Awaiting(async x => await x.ProcessWithResultAsync<object>(commandName, (JObject)null)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage("The json could not be converted to an object");
            }
        }

        [LoFu, Test]
        public async Task when_processing_the_command_with_or_without_result()
        {
            FakeCommandTypeCollection = new Mock<ICommandTypeCollection>();
            FakeServiceProvider = new Mock<IServiceProvider>();
            Subject = new CommandProcessor(FakeCommandTypeCollection.Object, FakeServiceProvider.Object);

            async Task should_invoke_the_correct_command_handler_for_commands_without_result()
            {
                var expectedCommandType = typeof(FakeCommand);
                var fakeCommandHandler = new Mock<ICommandHandler<FakeCommand>>();
                FakeCommandTypeCollection.Setup(x => x.GetType(expectedCommandType.Name)).Returns(expectedCommandType);
                FakeServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>))).Returns(fakeCommandHandler.Object);

                var result = await Subject.ProcessWithOrWithoutResultAsync(expectedCommandType.Name, "{}");

                fakeCommandHandler.Verify(x => x.HandleAsync(It.IsAny<FakeCommand>()));
                result.Should().Be(CommandResult.None);
            }

            async Task should_invoke_the_correct_command_handler_for_commands_with_result()
            {
                var expectedResult = new FakeResult();
                var expectedCommandType = typeof(FakeResultCommand);
                var fakeCommandHandler = new Mock<ICommandHandler<FakeResultCommand, FakeResult>>();
                fakeCommandHandler.Setup(x => x.HandleAsync(It.IsAny<FakeResultCommand>())).Returns(Task.FromResult(expectedResult));
                FakeCommandTypeCollection.Setup(x => x.GetType(expectedCommandType.Name)).Returns(expectedCommandType);
                FakeServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<FakeResultCommand, FakeResult>))).Returns(fakeCommandHandler.Object);

                var result = await Subject.ProcessWithOrWithoutResultAsync(expectedCommandType.Name, "{}");

                fakeCommandHandler.Verify(x => x.HandleAsync(It.IsAny<FakeResultCommand>()));
                result.Value.Should().Be(expectedResult);
            }
        }
        
        [LoFu, Test]
        public void when_get_commands()
        {
            FakeCommandTypeCollection = new Mock<ICommandTypeCollection>();
            Subject = new CommandProcessor(FakeCommandTypeCollection.Object, null);

            void should_get_all_types_from_the_cache()
            {
                Subject.GetCommands();

                FakeCommandTypeCollection.Verify(x => x.GetTypes());
            }
        }

        Mock<ICommandTypeCollection> FakeCommandTypeCollection;
        Mock<IServiceProvider> FakeServiceProvider;
        CommandProcessor Subject;
    }
}