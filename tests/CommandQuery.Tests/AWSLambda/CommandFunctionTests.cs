#if NETCOREAPP2_0
using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests.AWSLambda
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            Request = new APIGatewayProxyRequest();
            Context = new Mock<ILambdaContext>();
            Context.SetupGet(x => x.Logger).Returns(new Mock<ILambdaLogger>().Object);

            Use<Mock<ICommandProcessor>>();

            async Task should_invoke_the_command_processor()
            {
                var commandName = "FakeCommand";
                var content = "{}";

                await Subject.Handle(commandName, content);

                The<Mock<ICommandProcessor>>().Verify(x => x.ProcessAsync(commandName, content));
            }

            async Task base_method_should_not_handle_Exception()
            {
                var commandName = "FakeCommand";
                var content = "{}";

                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, content)).Throws(new Exception("fail"));

                Subject.Awaiting(async x => await x.Handle(commandName, content)).Should()
                    .Throw<Exception>()
                    .WithMessage("fail");
            }

            async Task should_handle_CommandValidationException()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, It.IsAny<string>())).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, Request, Context.Object);

                result.ShouldBeError("invalid");
            }

            async Task should_handle_Exception()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(commandName, Request, Context.Object);

                result.StatusCode.Should().Be(500);
                result.ShouldBeError("fail");
            }
        }

        APIGatewayProxyRequest Request;
        Mock<ILambdaContext> Context;
    }
}
#endif