using System.Net;
using System.Text;
using System.Text.Json;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.AzureFunctions.Tests
{
    public class CommandFunctionTests_HttpRequestData : LoFuTest<CommandFunction>
    {
        [SetUp]
        public async Task SetUp()
        {
            Clear();
            Use<JsonSerializerOptions>(null);

            Req = new FakeHttpRequestData(One<FunctionContext>());
            await Req.Body.WriteAsync(Encoding.UTF8.GetBytes("{}"));
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            Use<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                Req.Body.Position = 0;
                var result = await Subject.HandleAsync(CommandName, Req);
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(CommandName, (HttpRequestData)null);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_CommandProcessorException()
            {
                Req.Body.Position = 0;
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.HandleAsync(CommandName, Req);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.BadRequest);
            }

            async Task should_handle_CommandException()
            {
                Req.Body.Position = 0;
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandException("invalid"));

                var result = await Subject.HandleAsync(CommandName, Req);

                await result.ShouldBeErrorAsync("invalid", HttpStatusCode.BadRequest);
            }

            async Task should_handle_Exception()
            {
                Req.Body.Position = 0;
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(CommandName, Req);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.InternalServerError);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            Use<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeResultCommand));

            async Task should_return_the_result_from_the_command_processor()
            {
                Req.Body.Position = 0;
                var expected = new FakeResult();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(CommandName, Req);
                result.Body.Position = 0;

                result.StatusCode.Should().Be(HttpStatusCode.OK);
                result.Body.Length.Should().BeGreaterThan(0);
            }
        }

        HttpRequestData Req;
        string CommandName;
    }

    public class CommandFunctionTests_HttpRequest : LoFuTest<CommandFunction>
    {
        [SetUp]
        public async Task SetUp()
        {
            Clear();
            Use<JsonSerializerOptions>(null);

            var mock = new Mock<HttpRequest>();
            mock.SetupGet(x => x.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes("{}")));
            Req = mock.Object;
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            Use<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                Req.Body.Position = 0;
                var result = await Subject.HandleAsync(CommandName, Req) as IStatusCodeActionResult;
                result.StatusCode.Should().Be(200);
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(CommandName, (HttpRequest)null);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_CommandProcessorException()
            {
                Req.Body.Position = 0;
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.HandleAsync(CommandName, Req);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_CommandException()
            {
                Req.Body.Position = 0;
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandException("invalid"));

                var result = await Subject.HandleAsync(CommandName, Req);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                Req.Body.Position = 0;
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(CommandName, Req);

                result.ShouldBeError("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            Use<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeResultCommand));

            async Task should_return_the_result_from_the_command_processor()
            {
                Req.Body.Position = 0;
                var expected = new FakeResult();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(CommandName, Req);

                (result as IStatusCodeActionResult).StatusCode.Should().Be(200);
                (result as ObjectResult).Value.Should().NotBeNull();
            }
        }

        HttpRequest Req;
        string CommandName;
    }
}
