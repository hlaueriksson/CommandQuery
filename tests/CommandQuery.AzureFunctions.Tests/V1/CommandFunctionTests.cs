#if NET472
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CommandQuery.Exceptions;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V1
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [SetUp]
        public void SetUp()
        {
            Clear();
            Use<Mock<ICommandProcessor>>();
            Req = new HttpRequestMessage { Method = HttpMethod.Post, Content = new StringContent("{}") };
            Req.SetConfiguration(new HttpConfiguration());
            Logger = new FakeTraceWriter();
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            The<Mock<ICommandProcessor>>().Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Returns(Task.CompletedTask);
                var result = await Subject.Handle(CommandName, Req, Logger);

                result.StatusCode.Should().Be(200);
                result.Content.Should().BeNull();
            }

            async Task should_handle_CommandValidationException()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(CommandName, Req, Logger);

                await result.ShouldBeErrorAsync("invalid", 400);
            }

            async Task should_handle_Exception()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(CommandName, Req, Logger);

                await result.ShouldBeErrorAsync("fail", 500);
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

                var result = await Subject.Handle(CommandName, Req, Logger);

                result.StatusCode.Should().Be(200);
                result.Content.ReadAsAsync<FakeResult>().Should().NotBeNull();
            }
        }

        HttpRequestMessage Req;
        FakeTraceWriter Logger;
        string CommandName;
    }
}
#endif