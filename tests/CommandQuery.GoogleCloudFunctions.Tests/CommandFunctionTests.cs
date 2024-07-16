using System.Text;
using System.Text.Json;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.GoogleCloudFunctions.Tests
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            Use<JsonSerializerOptions>(null);
            Context = new DefaultHttpContext();
            Context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            Context.Response.Body = new MemoryStream();
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            Use<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                Context.Request.Body.Position = 0;
                Context.Response.Clear();
                await Subject.HandleAsync(CommandName, Context);

                Context.Response.StatusCode.Should().Be(200);
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(CommandName, null);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_CommandProcessorException()
            {
                Context.Request.Body.Position = 0;
                Context.Response.Clear();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandProcessorException("fail"));

                await Subject.HandleAsync(CommandName, Context);

                await Context.Response.ShouldBeErrorAsync("fail", 400);
            }

            async Task should_handle_CommandException()
            {
                Context.Request.Body.Position = 0;
                Context.Response.Clear();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandException("invalid"));

                await Subject.HandleAsync(CommandName, Context);

                await Context.Response.ShouldBeErrorAsync("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                Context.Request.Body.Position = 0;
                Context.Response.Clear();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                await Subject.HandleAsync(CommandName, Context);

                await Context.Response.ShouldBeErrorAsync("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            Use<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeResultCommand));

            async Task should_return_the_result_from_the_command_processor()
            {
                Context.Response.Clear();
                var expected = new FakeResult();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                await Subject.HandleAsync(CommandName, Context);

                Context.Response.StatusCode.Should().Be(200);
                Context.Response.Body.Length.Should().BeGreaterThan(0);
            }
        }

        HttpContext Context;
        string CommandName;
    }
}
