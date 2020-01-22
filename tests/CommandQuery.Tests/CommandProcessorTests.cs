using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests
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
                FakeServiceProvider.Setup(x => x.GetService(typeof(IEnumerable<ICommandHandler<FakeCommand>>))).Returns(new[] { fakeCommandHandler });

                var command = new FakeCommand();
                await Subject.ProcessAsync(command);

                command.Should().Be(expectedCommand);
            }

            void should_throw_exception_if_the_command_handler_is_not_found()
            {
                var command = new Mock<ICommand>().Object;

                Subject.Awaiting(x => x.ProcessAsync(command)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage($"The command handler for '{command}' could not be found");
            }

            void should_throw_exception_if_multiple_command_handlers_are_found()
            {
                var handlerType = typeof(ICommandHandler<FakeMultiCommand1>);
                var enumerableType = typeof(IEnumerable<ICommandHandler<FakeMultiCommand1>>);
                FakeServiceProvider.Setup(x => x.GetService(enumerableType)).Returns(new[] { new Mock<ICommandHandler<FakeMultiCommand1>>().Object, new Mock<ICommandHandler<FakeMultiCommand1>>().Object });

                var command = new FakeMultiCommand1();

                Subject.Awaiting(x => x.ProcessAsync(command)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage($"Multiple command handlers for '{handlerType}' was found");
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
                FakeServiceProvider.Setup(x => x.GetService(typeof(IEnumerable<ICommandHandler<FakeResultCommand, FakeResult>>))).Returns(new[] { fakeCommandHandler });

                var command = new FakeResultCommand();
                var result = await Subject.ProcessWithResultAsync(command);

                command.Should().Be(expectedCommand);
                result.Should().Be(expectedResult);
            }

            void should_throw_exception_if_the_command_handler_is_not_found()
            {
                var command = new Mock<ICommand<object>>().Object;

                Subject.Awaiting(x => x.ProcessWithResultAsync(command)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage($"The command handler for '{command}' could not be found");
            }

            void should_throw_exception_if_multiple_command_handlers_are_found()
            {
                var handlerType = typeof(ICommandHandler<FakeMultiResultCommand1, FakeResult>);
                var enumerableType = typeof(IEnumerable<ICommandHandler<FakeMultiResultCommand1, FakeResult>>);
                FakeServiceProvider.Setup(x => x.GetService(enumerableType)).Returns(new[] { new Mock<ICommandHandler<FakeMultiResultCommand1, FakeResult>>().Object, new Mock<ICommandHandler<FakeMultiResultCommand1, FakeResult>>().Object });

                var command = new FakeMultiResultCommand1();

                Subject.Awaiting(x => x.ProcessWithResultAsync(command)).Should()
                    .Throw<CommandProcessorException>()
                    .WithMessage($"Multiple command handlers for '{handlerType}' was found");
            }
        }

        [LoFu, Test]
        public void when_get_command_types()
        {
            FakeCommandTypeCollection = new Mock<ICommandTypeCollection>();
            Subject = new CommandProcessor(FakeCommandTypeCollection.Object, null);

            void should_get_all_types_from_the_cache()
            {
                Subject.GetCommandTypes();

                FakeCommandTypeCollection.Verify(x => x.GetTypes());
            }
        }

        [LoFu, Test]
        public void when_get_command_type()
        {
            FakeCommandTypeCollection = new Mock<ICommandTypeCollection>();
            Subject = new CommandProcessor(FakeCommandTypeCollection.Object, null);

            void should_get_the_type_from_the_cache()
            {
                var commandName = "name";

                Subject.GetCommandType(commandName);

                FakeCommandTypeCollection.Verify(x => x.GetType(commandName));
            }
        }

        Mock<ICommandTypeCollection> FakeCommandTypeCollection;
        Mock<IServiceProvider> FakeServiceProvider;
        CommandProcessor Subject;
    }
}