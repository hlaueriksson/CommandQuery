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
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            Json = JObject.Parse("{}");

            async Task should_invoke_the_command_processor()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Json)).Returns(Task.FromResult(CommandResult.None));

                var result = await Subject.Handle(CommandName, Json) as OkResult;

                result.StatusCode.Should().Be(200);
            }

            async Task should_handle_CommandValidationException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Json)).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(CommandName, Json) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Json)).Throws(new Exception("fail"));

                var result = await Subject.Handle(CommandName, Json) as ObjectResult;

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }
        
        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            Json = JObject.Parse("{}");

            async Task should_invoke_the_command_processor()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Json)).Returns(Task.FromResult(new CommandResult(new FakeResult())));

                var result = await Subject.Handle(CommandName, Json) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().NotBeNull();
            }
        }

        BaseCommandController Subject;
        Mock<ICommandProcessor> FakeCommandProcessor;
        Mock<HttpResponse> FakeHttpResponse;
        string CommandName;
        JObject Json;
    }
}