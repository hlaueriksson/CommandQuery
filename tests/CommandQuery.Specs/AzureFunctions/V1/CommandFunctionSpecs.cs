#if NET461
using System;
using System.Net.Http;
using System.Web.Http;
using CommandQuery.AzureFunctions;
using CommandQuery.Exceptions;
using Machine.Fakes;
using Machine.Specifications;

namespace CommandQuery.Specs.AzureFunctions.V1
{
    public class CommandFunctionSpecs
    {
        [Subject(typeof(CommandFunction))]
        public class when_handling_the_command : WithSubject<CommandFunction>
        {
            Establish context = () =>
            {
                Req = new HttpRequestMessage { Method = HttpMethod.Post, Content = new StringContent("") };
                Req.SetConfiguration(new HttpConfiguration());
                Log = new FakeTraceWriter();
            };

            It should_handle_CommandValidationException = () =>
            {
                var commandName = "FakeCommand";
                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new CommandValidationException("invalid"));

                var result = Subject.Handle(commandName, Req, Log).Result;

                result.ShouldBeError("invalid");
            };

            It should_handle_Exception = () =>
            {
                var commandName = "FakeCommand";
                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new Exception("fail"));

                var result = Subject.Handle(commandName, Req, Log).Result;

                result.ShouldBeError("fail");
            };

            static HttpRequestMessage Req;
            static FakeTraceWriter Log;
        }
    }
}
#endif