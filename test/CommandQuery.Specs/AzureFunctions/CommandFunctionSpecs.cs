using System;
using CommandQuery.AzureFunctions;
using CommandQuery.Exceptions;
using Machine.Fakes;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Specs.AzureFunctions
{
    public class CommandFunctionSpecs
    {
        [Subject(typeof(CommandFunction))]
        public class when_handling_the_command : WithSubject<CommandFunction>
        {
            Establish context = () =>
            {
                Req = new DefaultHttpRequest(new DefaultHttpContext());
                Log = new FakeTraceWriter();
            };

            It should_invoke_the_command_processor = () =>
            {
                var commandName = "FakeCommand";
                var content = "{}";

                Subject.Handle(commandName, content).Await();

                The<ICommandProcessor>().WasToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>()));
            };

            It base_method_should_not_handle_Exception = () =>
            {
                var commandName = "FakeCommand";
                var content = "{}";

                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new Exception("fail"));

                Catch.Exception(() => Subject.Handle(commandName, content).Await()).ShouldBeOfExactType<Exception>();
            };

            It v2_method_should_handle_CommandValidationException = async () =>
            {
                var commandName = "FakeCommand";
                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, Req, Log) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            };

            It v2_method_should_handle_Exception = async () =>
            {
                var commandName = "FakeCommand";
                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new Exception("fail"));

                var result = await Subject.Handle(commandName, Req, Log) as ObjectResult;

                result.StatusCode.ShouldEqual(500);
                result.ShouldBeError("fail");
            };

            static DefaultHttpRequest Req;
            static FakeTraceWriter Log;
        }
    }
}