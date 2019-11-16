#if NETCOREAPP2_2
using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
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
        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            _req = new DefaultHttpRequest(new DefaultHttpContext());
            _log = Use<ILogger>();

            Use<Mock<ICommandProcessor>>();

            async Task should_handle_CommandValidationException()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, It.IsAny<string>())).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, _req, _log) as BadRequestObjectResult;

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(commandName, _req, _log) as ObjectResult;

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        DefaultHttpRequest _req;
        ILogger _log;
    }
}
#endif