using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.Tests;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.AspNet.WebApi.Tests
{
    public class BaseCommandControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            FakeCommandProcessor = new Mock<ICommandProcessor>();
            Subject = new FakeCommandController(FakeCommandProcessor.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            Json = JObject.Parse("{}");

            async Task should_invoke_the_command_processor()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Json)).Returns(Task.FromResult(CommandResult.None));

                var result = await Subject.Handle(CommandName, Json);

                (await result.ExecuteAsync(CancellationToken.None)).Content.Should().BeNull();
            }

            async Task should_handle_Exception()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Json)).Throws(new Exception("fail"));

                var result = await Subject.Handle(CommandName, Json) as ExceptionResult;

                await result.ShouldBeErrorAsync("fail");
            }
        }
        
        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            Json = JObject.Parse("{}");

            async Task should_invoke_the_command_processor()
            {
                FakeCommandProcessor.Setup(x => x.ProcessWithOrWithoutResultAsync(CommandName, Json)).Returns(Task.FromResult(new CommandResult(new FakeResult())));

                var result = await Subject.Handle(CommandName, Json);

                (await result.ExecuteAsync(CancellationToken.None)).Content.Should().NotBeNull();
            }
        }

        BaseCommandController Subject;
        Mock<ICommandProcessor> FakeCommandProcessor;
        string CommandName;
        JObject Json;
    }
}