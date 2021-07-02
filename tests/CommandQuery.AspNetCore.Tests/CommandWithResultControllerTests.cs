using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AspNetCore.Tests
{
    public class CommandWithResultControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            Subject = new CommandWithResultController<FakeResultCommand, FakeResult>(FakeCommandProcessor.Object, null);
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            async Task should_return_the_result_from_the_command_processor()
            {
                var expected = new FakeResult();
                FakeCommandProcessor.Setup(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(new FakeResultCommand()) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }

            async Task should_handle_CommandProcessorException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.HandleAsync(new FakeResultCommand());

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_CommandException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>())).Throws(new CommandException("invalid"));

                var result = await Subject.HandleAsync(new FakeResultCommand());

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(new FakeResultCommand());

                result.ShouldBeError("fail", 500);
            }
        }

        Mock<ICommandProcessor> FakeCommandProcessor;
        private CommandWithResultController<FakeResultCommand, FakeResult> Subject;
    }
}