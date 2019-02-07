#if NET471
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CommandQuery.AzureFunctions;
using CommandQuery.Exceptions;
using LoFuUnit.AutoMoq;
using Moq;

namespace CommandQuery.Tests.AzureFunctions.V1
{
    public class CommandFunctionTests : LoFuTest<CommandFunction>
    {
        public async Task when_handling_the_command()
        {
            Req = new HttpRequestMessage { Method = HttpMethod.Post, Content = new StringContent("") };
            Req.SetConfiguration(new HttpConfiguration());
            Log = new FakeTraceWriter();

            Use<Mock<ICommandProcessor>>();

            async Task should_handle_CommandValidationException()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, It.IsAny<string>())).Throws(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, Req, Log);

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                var commandName = "FakeCommand";
                The<Mock<ICommandProcessor>>().Setup(x => x.ProcessAsync(commandName, It.IsAny<string>())).Throws(new Exception("fail"));

                var result = await Subject.Handle(commandName, Req, Log);

                await result.ShouldBeErrorAsync("fail");
            }
        }

        HttpRequestMessage Req;
        FakeTraceWriter Log;
    }
}
#endif