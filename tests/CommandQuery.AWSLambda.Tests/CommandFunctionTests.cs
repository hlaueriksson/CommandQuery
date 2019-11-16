using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
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
            Context = new Mock<ILambdaContext>();
            Context.SetupGet(x => x.Logger).Returns(new Mock<ILambdaLogger>().Object);
            Request = new APIGatewayProxyRequest { Body = "{}" };
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";

            async Task should_invoke_the_command_processor()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Request.Body)).Returns(Task.FromResult(CommandResult.None));

                var result = await Subject.Handle(CommandName, Request, Context.Object);

                result.Body.Should().BeNull();
            }

            async Task should_handle_Exception()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Request.Body)).Throws(new Exception("fail"));

                var result = await Subject.Handle(CommandName, Request, Context.Object);

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }    
        
        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";

            async Task should_invoke_the_command_processor()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Request.Body)).Returns(Task.FromResult(new CommandResult(new FakeResult())));

                var result = await Subject.Handle(CommandName, Request, Context.Object);

                result.Body.Should().NotBeNull();
            }

            async Task should_handle_Exception()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Request.Body)).Throws(new Exception("fail"));

                var result = await Subject.Handle(CommandName, Request, Context.Object);

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        APIGatewayProxyRequest Request;
        Mock<ILambdaContext> Context;
        string CommandName;
    }
}