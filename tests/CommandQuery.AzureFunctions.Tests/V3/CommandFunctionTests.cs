#if NETCOREAPP3_1
using System;
using System.IO;
using System.Text;
using System.Threading;
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
using Newtonsoft.Json;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V3
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            Use<Mock<ICommandProcessor>>();
            Use<JsonSerializerSettings>(null);
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
                var result = await Subject.HandleAsync(CommandName, Req, Logger) as OkResult;

                result.StatusCode.Should().Be(200);
            }

            async Task should_throw_when_request_is_null()
            {
                Subject.Awaiting(x => x.HandleAsync(CommandName, null, Logger))
                    .Should().Throw<ArgumentNullException>();
            }

            async Task should_handle_CommandProcessorException()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.HandleAsync(CommandName, Req, Logger);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_CommandException()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandException("invalid"));

                var result = await Subject.HandleAsync(CommandName, Req, Logger);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(CommandName, Req, Logger);

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
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeResultCommand>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleAsync(CommandName, Req, Logger) as OkObjectResult;

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
