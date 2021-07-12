using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CommandQuery.GoogleCloudFunctions.Tests
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            Use<Mock<ICommandProcessor>>();
            Use<JsonSerializerOptions>(null);
            Context = new DefaultHttpContext();
            Context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            Context.Response.Body = new MemoryStream();
            Logger = Use<ILogger>();
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            The<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                Context.Response.Clear();
                await Subject.HandleAsync(CommandName, Context, Logger);

                Context.Response.StatusCode.Should().Be(200);
            }

            async Task should_throw_when_request_is_null()
            {
                Subject.Awaiting(x => x.HandleAsync(CommandName, null, Logger))
                    .Should().Throw<ArgumentNullException>();
            }

            async Task should_handle_CommandProcessorException()
            {
                Context.Response.Clear();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new CommandProcessorException("fail"));

                await Subject.HandleAsync(CommandName, Context, Logger);

                await Context.Response.ShouldBeErrorAsync("fail", 400);
            }

            async Task should_handle_CommandException()
            {
                Context.Response.Clear();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new CommandException("invalid"));

                await Subject.HandleAsync(CommandName, Context, Logger);

                await Context.Response.ShouldBeErrorAsync("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                Context.Response.Clear();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new Exception("fail"));

                await Subject.HandleAsync(CommandName, Context, Logger);

                await Context.Response.ShouldBeErrorAsync("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            The<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeResultCommand));

            async Task should_return_the_result_from_the_command_processor()
            {
                Context.Response.Clear();
                var expected = new FakeResult();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>())).Returns(Task.FromResult(expected));

                await Subject.HandleAsync(CommandName, Context, Logger);

                Context.Response.StatusCode.Should().Be(200);
                Context.Response.Body.Length.Should().BeGreaterThan(0);
            }
        }

        HttpContext Context;
        ILogger Logger;
        string CommandName;
    }
}
