#if NETCOREAPP2_2
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V2
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            Use<Mock<ICommandProcessor>>();
            Req = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"))
            };
            Logger = Use<ILogger>();
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            The<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                var result = await Subject.Handle(CommandName, Req, Logger) as OkResult;

                result.StatusCode.Should().Be(200);
            }

            async Task should_handle_CommandProcessorException()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.Handle(CommandName, Req, Logger);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_CommandValidationException()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(CommandName, Req, Logger);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(CommandName, Req, Logger);

                result.ShouldBeError("fail", 500);
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            The<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeResultCommand));

            async Task should_return_the_result_from_the_command_processor()
            {
                var expected = new FakeResult();
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>())).Returns(Task.FromResult(expected));

                var result = await Subject.Handle(CommandName, Req, Logger) as OkObjectResult;

                result.StatusCode.Should().Be(200);
                result.Value.Should().Be(expected);
            }
        }

        DefaultHttpRequest Req;
        ILogger Logger;
        string CommandName;
    }
}
#endif