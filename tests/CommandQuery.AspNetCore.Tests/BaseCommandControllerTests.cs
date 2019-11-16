using System;
using System.Reflection;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.AspNetCore.Tests
{
    public class BaseCommandControllerTests
    {
        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            FakeHttpResponse = new Mock<HttpResponse>();

            Subject = new FakeCommandController(FakeCommandProcessor.Object)
            {
                ControllerContext = Fake.ControllerContext(fakeHttpResponse: FakeHttpResponse)
            };

            async Task should_invoke_the_command_processor()
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");

                await Subject.Handle(commandName, json);

                FakeCommandProcessor.Verify(x => x.ProcessWithOrWithoutResultAsync(commandName, json));
            }

            async Task should_handle_CommandValidationException()
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessWithOrWithoutResultAsync(commandName, json)).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, json) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessWithOrWithoutResultAsync(commandName, json)).Throws(new Exception("fail"));

                var result = await Subject.Handle(commandName, json) as ObjectResult;

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        [LoFu, Test]
        public async Task when_using_the_real_CommandProcessor()
        {
            var fakeCommandHandler = new Mock<ICommandHandler<FakeCommand>>();
            var fakeCommandWithResultHandler = new Mock<ICommandHandler<FakeResultCommand, FakeResult>>();

            var mock = new Mock<IServiceProvider>();
            mock.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>))).Returns(fakeCommandHandler.Object);
            mock.Setup(x => x.GetService(typeof(ICommandHandler<FakeResultCommand, FakeResult>))).Returns(fakeCommandWithResultHandler.Object);

            Subject = new FakeCommandController(new CommandProcessor(new CommandTypeCollection(typeof(FakeCommand).GetTypeInfo().Assembly), mock.Object))
            {
                ControllerContext = Fake.ControllerContext()
            };

            async Task should_work()
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");

                var result = await Subject.Handle(commandName, json) as OkResult;

                result.Should().NotBeNull();
            }

            async Task should_handle_errors()
            {
                var commandName = "NotFoundCommand";
                var json = JObject.Parse("{}");

                var result = await Subject.Handle(commandName, json) as BadRequestObjectResult;

                result.ShouldBeError("The command type 'NotFoundCommand' could not be found");
            }
        }

        Mock<ICommandProcessor> FakeCommandProcessor;
        Mock<HttpResponse> FakeHttpResponse;
        BaseCommandController Subject;
    }
}