using System;
using System.Reflection;
using CommandQuery.AspNetCore;
using CommandQuery.Exceptions;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using It = Machine.Specifications.It;

namespace CommandQuery.Specs.AspNetCore
{
    public class BaseCommandControllerSpecs
    {
        [Subject(typeof(BaseCommandController))]
        public class when_handling_the_command
        {
            Establish context = () =>
            {
                FakeCommandProcessor = new Mock<ICommandProcessor>();
                FakeHttpResponse = new Mock<HttpResponse>();

                Subject = new FakeCommandController(FakeCommandProcessor.Object)
                {
                    ControllerContext = Fake.ControllerContext(fakeHttpResponse: FakeHttpResponse)
                };
            };

            It should_invoke_the_command_processor = async () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");

                await Subject.Handle(commandName, json);

                FakeCommandProcessor.Verify(x => x.ProcessAsync(commandName, json));
            };

            private It should_handle_CommandValidationException = async () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessAsync(commandName, json)).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, json) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            };

            It should_handle_Exception = async () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessAsync(commandName, json)).Throws(new Exception("fail"));

                var result = await Subject.Handle(commandName, json) as ObjectResult;

                result.StatusCode.ShouldEqual(500);
                result.ShouldBeError("fail");
            };

            static Mock<ICommandProcessor> FakeCommandProcessor;
            static Mock<HttpResponse> FakeHttpResponse;
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

                Subject = new FakeCommandController(new CommandProcessor(new CommandTypeCollection(typeof(FakeCommand).GetTypeInfo().Assembly), mock.Object))
                {
                    ControllerContext = Fake.ControllerContext()
                };
            };

            It should_work = async () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");

                var result = await Subject.Handle(commandName, json) as OkResult;

                result.ShouldNotBeNull();
            };

            It should_handle_errors = async () =>
            {
                var commandName = "NotFoundCommand";
                var json = JObject.Parse("{}");

                var result = await Subject.Handle(commandName, json) as BadRequestObjectResult;

                result.ShouldBeError("The command type 'NotFoundCommand' could not be found");
            };

            static BaseCommandController Subject;
        }
    }
}