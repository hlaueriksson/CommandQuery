#if NET472
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CommandQuery.Exceptions;
using LoFuUnit.AutoMoq;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V1
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        [LoFu, Test]
        public async Task when_handling_the_command()
        {
            _req = new HttpRequestMessage { Method = HttpMethod.Post, Content = new StringContent("") };
            _req.SetConfiguration(new HttpConfiguration());
            _log = new FakeTraceWriter();

            Use<Mock<ICommandProcessor>>();

            async Task should_handle_CommandValidationException()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, It.IsAny<string>())).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, _req, _log);

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(commandName, _req, _log);

                await result.ShouldBeErrorAsync("fail");
            }
        }

        HttpRequestMessage _req;
        FakeTraceWriter _log;
    }
}
#endif