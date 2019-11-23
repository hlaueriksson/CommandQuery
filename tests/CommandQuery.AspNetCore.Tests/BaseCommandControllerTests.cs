using System;
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
        [SetUp]
        public void SetUp()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            FakeHttpResponse = new Mock<HttpResponse>();
            Subject = new FakeCommandController(FakeCommandProcessor.Object)
            {
                ControllerContext = Fake.ControllerContext(fakeHttpResponse: FakeHttpResponse)
            };
            Json = JObject.Parse("{}");
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            FakeCommandProcessor.Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                var result = await Subject.Handle(CommandName, Json) as OkResult;

                result.StatusCode.Should().Be(200);
            }

            async Task should_handle_CommandValidationException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(CommandName, Json);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(CommandName, Json);

                result.ShouldBeError("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            FakeCommandProcessor.Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeResultCommand));

            async Task should_return_the_result_from_the_command_processor()
            {
                var expected = new FakeResult();
                FakeCommandProcessor.Setup(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>())).Returns(Task.FromResult(expected));

                var result = await Subject.Handle(CommandName, Json) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }
        }

        BaseCommandController Subject;
        Mock<ICommandProcessor> FakeCommandProcessor;
        Mock<HttpResponse> FakeHttpResponse;
        string CommandName;
        JObject Json;
    }
}