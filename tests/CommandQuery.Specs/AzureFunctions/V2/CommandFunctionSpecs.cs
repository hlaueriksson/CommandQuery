#if NETCOREAPP2_0
using System;
using CommandQuery.AzureFunctions;
using CommandQuery.Exceptions;
using Machine.Fakes;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.Specs.AzureFunctions.V2
{
    public class CommandFunctionSpecs
    {
        [Subject(typeof(CommandFunction))]
        public class when_handling_the_command : WithSubject<CommandFunction>
        {
            Establish context = () =>
            {
                Req = new DefaultHttpRequest(new DefaultHttpContext());
                Log = An<ILogger>();
            };

            It should_handle_CommandValidationException = async () =>
            {
                var commandName = "FakeCommand";
                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, Req, Log) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            };

            It should_handle_Exception = async () =>
            {
                var commandName = "FakeCommand";
                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new Exception("fail"));

                var result = await Subject.Handle(commandName, Req, Log) as ObjectResult;

                result.StatusCode.ShouldEqual(500);
                result.ShouldBeError("fail");
            };

            static DefaultHttpRequest Req;
            static ILogger Log;
        }
    }
}
#endif