using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Tracing;
using CommandQuery.Exceptions;
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
            Json = JObject.Parse("{}");
        }

        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            CommandName = "FakeCommand";
            FakeCommandProcessor.Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeCommand));

            async Task should_invoke_the_command_processor()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Returns(Task.CompletedTask);

                var result = await Subject.Handle(CommandName, Json);

                (await result.ExecuteAsync(CancellationToken.None)).StatusCode.Should().Be(HttpStatusCode.OK);
            }

            async Task should_handle_CommandProcessorException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new CommandProcessorException("fail"));

                var result = await Subject.Handle(CommandName, Json);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.BadRequest);
            }
            
            async Task should_handle_CommandException()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new CommandException("invalid"));

                var result = await Subject.Handle(CommandName, Json);

                await result.ShouldBeErrorAsync("invalid", HttpStatusCode.BadRequest);
            }

            async Task should_handle_Exception()
            {
                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(CommandName, Json);

                await result.ShouldBeErrorAsync("fail", HttpStatusCode.InternalServerError);
            }
            
            async Task should_log_errors()
            {
                var fakeTraceWriter = new Mock<ITraceWriter>();
                var subject = new FakeCommandController(FakeCommandProcessor.Object, fakeTraceWriter.Object)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };

                FakeCommandProcessor.Setup(x => x.ProcessAsync(It.IsAny<FakeCommand>())).Throws(new Exception("fail"));

                await subject.Handle(CommandName, Json);

                fakeTraceWriter.Verify(x => x.Trace(It.IsAny<HttpRequestMessage>(), "UnhandledCommandException", TraceLevel.Error, It.IsAny<Action<TraceRecord>>()));
            }
        }
        
        [LoFu, Test]
        public async Task when_handling_the_command_with_result()
        {
            CommandName = "FakeResultCommand";
            FakeCommandProcessor.Setup(x => x.GetCommandType(CommandName)).Returns(typeof(FakeResultCommand));

            async Task should_return_the_result_from_the_command_processor()
            {
                var expected = new FakeResult();
                FakeCommandProcessor.Setup(x => x.ProcessWithResultAsync(It.IsAny<FakeResultCommand>())).Returns(Task.FromResult(expected));

                var result = await Subject.Handle(CommandName, Json) as OkNegotiatedContentResult<object>;

                (await result.ExecuteAsync(CancellationToken.None)).StatusCode.Should().Be(HttpStatusCode.OK);
                result.Content.Should().Be(expected);
            }
        }

        BaseCommandController Subject;
        Mock<ICommandProcessor> FakeCommandProcessor;
        string CommandName;
        JObject Json;
    }
}
