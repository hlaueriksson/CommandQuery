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
            Req = new HttpRequestMessage { Method = HttpMethod.Post, Content = new StringContent("") };
            Req.SetConfiguration(new HttpConfiguration());
            _log = new FakeTraceWriter();
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";

            async Task should_invoke_the_command_processor()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, It.IsAny<string>())).Returns(Task.FromResult(CommandResult.None));

                var result = await Subject.Handle(CommandName, Req, _log);

                result.IsSuccessStatusCode.Should().BeTrue();
                result.Content.Should().BeNull();
            }     
            
            async Task should_handle_CommandValidationException()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, It.IsAny<string>())).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(CommandName, Req, _log);

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(CommandName, Req, _log);

                await result.ShouldBeErrorAsync("fail");
            }
        }  
        
        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeCommand";

            async Task should_invoke_the_command_processor_and_return_the_result()
            {
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, It.IsAny<string>())).Returns(Task.FromResult(new CommandResult(new FakeResult())));

                var result = await Subject.Handle(CommandName, Req, _log);

                result.IsSuccessStatusCode.Should().BeTrue();
                result.Content.Should().NotBeNull();
            }
        }

        HttpRequestMessage Req;
        FakeTraceWriter _log;
        string CommandName;
    }
}
#endif