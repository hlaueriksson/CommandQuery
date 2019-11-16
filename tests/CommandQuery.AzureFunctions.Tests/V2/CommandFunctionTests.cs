#if NETCOREAPP2_2
using System;
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
            Req = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Use<ILogger>();
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";

            async Task should_invoke_the_command_processor()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, It.IsAny<string>())).Returns(Task.FromResult(CommandResult.None));

                var result = await Subject.Handle(CommandName, Req, _log) as OkResult;

                result.Should().NotBeNull();
            }

            async Task should_handle_CommandValidationException()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(commandName, It.IsAny<string>())).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, Req, _log) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(commandName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(commandName, Req, _log) as ObjectResult;

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }     
        
        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";

            async Task should_invoke_the_command_processor_and_return_the_result()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, It.IsAny<string>())).Returns(Task.FromResult(new CommandResult(new FakeResult())));

                var result = await Subject.Handle(CommandName, Req, _log) as OkObjectResult;

                result.Should().NotBeNull();
            }
        }

        DefaultHttpRequest Req;
        ILogger _log;
        string CommandName;
    }
}
#endif