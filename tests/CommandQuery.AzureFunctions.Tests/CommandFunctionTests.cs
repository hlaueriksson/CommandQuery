using System.Net;
using System.Text;
using System.Text.Json;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [SetUp]
        public async Task SetUp()
        {
            Clear();
            Use<Mock<ICommandProcessor>>();
            Use<JsonSerializerOptions>(null);

            var context = new Mock<FunctionContext>();
            Req = new FakeHttpRequestData(context.Object);
            await Req.Body.WriteAsync(Encoding.UTF8.GetBytes("{}"));

            Logger = Use<ILogger>();
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            The<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                Req.Body.Position = 0;
                var result = await Subject.HandleAsync(CommandName, Req, Logger);
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(CommandName, null, Logger);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_CommandProcessorException()
            {
                Req.Body.Position = 0;
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.HandleAsync(CommandName, Req, Logger);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.BadRequest);
            }

            async Task should_handle_CommandException()
            {
                Req.Body.Position = 0;
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandException("invalid"));

                var result = await Subject.HandleAsync(CommandName, Req, Logger);

                await result.ShouldBeErrorAsync("invalid", HttpStatusCode.BadRequest);
            }

            async Task should_handle_Exception()
            {
                Req.Body.Position = 0;
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(CommandName, Req, Logger);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.InternalServerError);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            The<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeResultCommand));

            async Task should_return_the_result_from_the_command_processor()
            {
                Req.Body.Position = 0;
                var expected = new FakeResult();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(CommandName, Req, Logger);
                result.Body.Position = 0;

                result.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Body.Length.Should().BeGreaterThan(0);
            }
        }

        HttpRequestData Req;
        ILogger Logger;
        string CommandName;
    }
}
