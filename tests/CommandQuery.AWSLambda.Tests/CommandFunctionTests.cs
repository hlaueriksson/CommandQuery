using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AWSLambda.Tests
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            Use<Mock<ICommandProcessor>>();
            Use<JsonSerializerOptions>(null);
            Logger = new Mock<ILambdaLogger>().Object;
            Request = new APIGatewayProxyRequest { Body = "{}" };
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            The<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                var result = await Subject.HandleAsync(CommandName, Request, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Should().BeNull();
            }

            async Task should_throw_when_request_is_null()
            {
                Func<Task> act = () => Subject.HandleAsync(CommandName, null, Logger);
                await act.Should().ThrowAsync<ArgumentNullException>();
            }

            async Task should_handle_CommandProcessorException()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.HandleAsync(CommandName, Request, Logger);

                result.ShouldBeError("fail", 400);
            }

            async Task should_handle_CommandException()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new CommandException("invalid"));

                var result = await Subject.HandleAsync(CommandName, Request, Logger);

                result.ShouldBeError("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>(), It.IsAny<CancellationToken>())).Throws(new Exception("fail"));

                var result = await Subject.HandleAsync(CommandName, Request, Logger);

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

                var result = await Subject.HandleAsync(CommandName, Request, Logger);

                result.StatusCode.Should().Be(200);
                result.Body.Should().NotBeNull();
            }
        }

        APIGatewayProxyRequest Request;
        ILambdaLogger Logger;
        string CommandName;
    }
}
