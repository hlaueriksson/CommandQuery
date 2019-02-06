using System;
using Machine.Specifications;
using Moq;
using Newtonsoft.Json.Linq;
using It = Machine.Specifications.It;

namespace CommandQuery.Specs._
{
    public class CommandProcessorSpecs
    {
        [Subject(typeof(CommandProcessor))]
        public class when_processing_the_command
        {
            Establish context = () =>
            {
                FakeCommandTypeCollection = new Mock<ICommandTypeCollection>();
                FakeServiceProvider = new Mock<IServiceProvider>();
                Subject = new CommandProcessor(FakeCommandTypeCollection.Object, FakeServiceProvider.Object);
            };

            It should_invoke_the_correct_command_handler = async () =>
            {
                var command = new FakeCommand();
                var fakeCommandHandler = new Mock<ICommandHandler<FakeCommand>>();
                FakeServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>))).Returns(fakeCommandHandler.Object);

                await Subject.ProcessAsync(command);

                fakeCommandHandler.Verify(x => x.HandleAsync(command));
            };

            It should_throw_exception_if_the_command_handler_is_not_found = () =>
            {
                var command = new Mock<ICommand>().Object;

                var exception = Catch.Exception(() => Subject.ProcessAsync(command).Await());

                exception.ShouldContainErrorMessage($"The command handler for '{command}' could not be found");
            };

            It should_throw_exception_if_the_command_type_is_not_found = () =>
            {
                var commandName = "NotFoundCommand";
                var json = JObject.Parse("{}");

                var exception = Catch.Exception(() => Subject.ProcessAsync(commandName, json).Await());

                exception.ShouldContainErrorMessage("The command type 'NotFoundCommand' could not be found");
            };

            It should_throw_exception_if_the_json_is_invalid = () =>
            {
                var commandName = "FakeCommand";
                FakeCommandTypeCollection.Setup(x => x.GetType(commandName)).Returns(typeof(FakeCommand));

                var exception = Catch.Exception(() => Subject.ProcessAsync(commandName, (JObject)null).Await());

                exception.ShouldContainErrorMessage("The json could not be converted to an object");
            };

            static Mock<ICommandTypeCollection> FakeCommandTypeCollection;
            static Mock<IServiceProvider> FakeServiceProvider;
            static CommandProcessor Subject;
        }
    }
}