using System;
using CommandQuery.AzureFunctions;
using Machine.Fakes;
using Machine.Specifications;

namespace CommandQuery.Specs.AzureFunctions
{
    public class CommandFunctionSpecs
    {
        [Subject(typeof(CommandFunction))]
        public class when_handling_the_command : WithSubject<CommandFunction>
        {
            It should_invoke_the_command_processor = () =>
            {
                var commandName = "FakeCommand";
                var content = "{}";

                Subject.Handle(commandName, content).Await();

                The<ICommandProcessor>().WasToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>()));
            };

            It should_not_handle_Exception = () =>
            {
                var commandName = "FakeCommand";
                var content = "{}";

                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new Exception("fail"));

                Catch.Exception(() => Subject.Handle(commandName, content).Await()).ShouldBeOfExactType<Exception>();
            };
        }
    }
}