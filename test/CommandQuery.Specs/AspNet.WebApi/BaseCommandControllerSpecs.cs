#if NET461
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.AspNet.WebApi;
using CommandQuery.Exceptions;
using Machine.Specifications;
using Moq;
using Newtonsoft.Json.Linq;
using ExceptionResult = System.Web.Http.Results.ExceptionResult;
using It = Machine.Specifications.It;

namespace CommandQuery.Specs.AspNet.WebApi
{
    public class BaseCommandControllerSpecs
    {
        [Subject(typeof(BaseCommandController))]
        public class when_handling_the_command
        {
            Establish context = () =>
            {
                FakeCommandProcessor = new Mock<ICommandProcessor>();

                Subject = new FakeCommandController(FakeCommandProcessor.Object)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };
            };

            It should_invoke_the_command_processor = () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");

                Subject.Handle(commandName, json).Await();

                FakeCommandProcessor.Verify(x => x.ProcessAsync(commandName, json));
            };

            It should_handle_CommandValidationException = () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessAsync(commandName, json)).Throws(new CommandValidationException("invalid"));

                var result = Subject.Handle(commandName, json).Result as BadRequestErrorMessageResult;

                result.ShouldBeError("invalid");
            };

            It should_handle_Exception = () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessAsync(commandName, json)).Throws(new Exception("fail"));

                var result = Subject.Handle(commandName, json).Result as ExceptionResult;

                result.ShouldBeError("fail");
            };

            static Mock<ICommandProcessor> FakeCommandProcessor;
            static BaseCommandController Subject;
        }

        [Subject(typeof(BaseCommandController))]
        public class when_using_the_real_CommandProcessor
        {
            Establish context = () =>
            {
                var fakeCommandHandler = new Mock<ICommandHandler<FakeCommand>>();

                var mock = new Mock<IServiceProvider>();
                mock.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>))).Returns(fakeCommandHandler.Object);

                Subject = new FakeCommandController(new CommandProcessor(new CommandTypeCollection(typeof(FakeCommand).Assembly), mock.Object))
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };
            };

            It should_work = () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");

                var result = Subject.Handle(commandName, json).Result as OkResult;

                result.ShouldNotBeNull();
            };

            It should_handle_errors = () =>
            {
                var commandName = "NotFoundCommand";
                var json = JObject.Parse("{}");

                var result = Subject.Handle(commandName, json).Result as BadRequestErrorMessageResult;

                result.ShouldBeError("The command type 'NotFoundCommand' could not be found");
            };

            static BaseCommandController Subject;
        }
    }
}
#endif