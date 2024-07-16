using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore.Tests
{
    public class CommandControllerWithoutResultTests
    {
        [SetUp]
        public void SetUp()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            Subject = new CommandController<FakeCommand>(FakeCommandProcessor.Object, new Mock<ILogger<CommandController<FakeCommand>>>().Object);
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            async Task should_invoke_the_command_processor()
            {
                var result = await Subject.HandleAsync(new FakeCommand(), CancellationToken.None) as OkResult;

                result.StatusCode.Should().Be(200);
            }

            async Task should_handle_CommandProcessorException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.HandleAsync(new FakeCommand(), CancellationToken.None);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_CommandException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandException("invalid"));

                var result = await Subject.HandleAsync(new FakeCommand(), CancellationToken.None);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(new FakeCommand(), CancellationToken.None);

                result.ShouldBeError("fail", 500);
            }
        }

        Mock<ICommandProcessor> FakeCommandProcessor;
        private CommandController<FakeCommand> Subject;
    }
}
