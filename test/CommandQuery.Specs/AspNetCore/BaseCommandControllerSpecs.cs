using System;
using System.Reflection;
using CommandQuery.AspNetCore;
using CommandQuery.Exceptions;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
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

            It should_handle_CommandValidationException = async () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessAsync(commandName, json)).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, json);

                result.ShouldEqual("Validation error: invalid");

                FakeHttpResponse.VerifySet(x => x.StatusCode = 400);
            };

            It should_handle_Exception = async () =>
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessAsync(commandName, json)).Throws(new Exception("fail"));

                var result = await Subject.Handle(commandName, json);

                result.ShouldEqual("Error: fail");

                FakeHttpResponse.VerifySet(x => x.StatusCode = 500);
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

                var result = await Subject.Handle(commandName, json) as string;

                result.ShouldBeEmpty();
            };

            It should_handle_errors = async () =>
            {
                var commandName = "NotFoundCommand";
                var json = JObject.Parse("{}");

                var result = await Subject.Handle(commandName, json) as string;

                result.ShouldEqual("The command type 'NotFoundCommand' could not be found");
            };

            static BaseCommandController Subject;
        }
    }
}