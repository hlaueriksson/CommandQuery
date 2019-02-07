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
                var fakeCommandHandler = new FakeCommandHandler(x => Expected = x);
                FakeServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>))).Returns(fakeCommandHandler);

                var command = new FakeCommand();

                await Subject.ProcessAsync(command);

                Expected.Should().Be(command);
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

        Mock<ICommandTypeCollection> FakeCommandTypeCollection;
        Mock<IServiceProvider> FakeServiceProvider;
        CommandProcessor Subject;
        FakeCommand Expected;
    }
}