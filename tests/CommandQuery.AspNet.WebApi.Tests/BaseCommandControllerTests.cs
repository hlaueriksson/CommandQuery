using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.AspNet.WebApi.Tests
{
    public class BaseCommandControllerTests
    {
        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();

            Subject = new FakeCommandController(FakeCommandProcessor.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            async Task should_invoke_the_command_processor()
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");

                await Subject.Handle(commandName, json);

                FakeCommandProcessor.Verify(x => x.ProcessAsync(commandName, json));
            }

            async Task should_handle_CommandValidationException()
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessAsync(commandName, json)).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, json) as BadRequestErrorMessageResult;

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                var commandName = "FakeCommand";
                var json = JObject.Parse("{}");
                FakeCommandProcessor.Setup(x => x.ProcessAsync(commandName, json)).Throws(new Exception("fail"));

                var result = await Subject.Handle(commandName, json) as ExceptionResult;

                await result.ShouldBeErrorAsync("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_command_with_real_CommandProcessor()
        {
            var fakeCommandHandler = new FakeCommandHandler(_ => { });

            var mock = new Mock<IServiceProvider>();
            mock.Setup(x => x.GetService(typeof(ICommandHandler<FakeCommand>))).Returns(fakeCommandHandler);

            Subject = new FakeCommandController(new CommandProcessor(new CommandTypeCollection(typeof(FakeCommand).Assembly), mock.Object))
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
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

                var result = await Subject.Handle(commandName, json) as BadRequestErrorMessageResult;

                await result.ShouldBeErrorAsync("The command type 'NotFoundCommand' could not be found");
            }
        }

        Mock<ICommandProcessor> FakeCommandProcessor;
        BaseCommandController Subject;
    }
}