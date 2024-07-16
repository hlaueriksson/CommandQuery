using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AspNetCore.Tests
{
    public class CommandControllerWithResultTests
    {
        [SetUp]
        public void SetUp()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            Subject = new CommandController<FakeResultCommand, FakeResult>(FakeCommandProcessor.Object, new Mock<ILogger<CommandController<FakeResultCommand, FakeResult>>>().Object);
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            async Task should_return_the_result_from_the_command_processor()
            {
                var expected = new FakeResult();
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(new FakeResultCommand(), CancellationToken.None) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }

            async Task should_handle_CommandProcessorException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.HandleAsync(new FakeResultCommand(), CancellationToken.None);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_CommandException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandException("invalid"));

                var result = await Subject.HandleAsync(new FakeResultCommand(), CancellationToken.None);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(new FakeResultCommand(), CancellationToken.None);

                result.ShouldBeError("fail", 500);
            }
        }

        Mock<ICommandProcessor> FakeCommandProcessor;
        private CommandController<FakeResultCommand, FakeResult> Subject;
    }
}
