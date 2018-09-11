#if NETCOREAPP2_0
using System;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CommandQuery.AWSLambda;
using CommandQuery.Exceptions;
using Machine.Fakes;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace CommandQuery.Specs.AWSLambda
{
    public class CommandFunctionSpecs
    {
        [Subject(typeof(CommandFunction))]
        public class when_handling_the_command : WithSubject<CommandFunction>
        {
            It should_invoke_the_command_processor = () =>
            {
                var commandName = "FakeCommand";
                var content = "{}";

                Subject.Handle(commandName, content).Await();

                The<ICommandProcessor>().WasToldTo(x => x.ProcessAsync(commandName, content));
            };

            It base_method_should_not_handle_Exception = () =>
            {
                var commandName = "FakeCommand";
                var content = "{}";

                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, content)).Throw(new Exception("fail"));

                Catch.Exception(() => Subject.Handle(commandName, content).Await()).ShouldBeOfExactType<Exception>();
            };

            private Establish context = () =>
            {
                Request = new APIGatewayProxyRequest();
                Context = new Mock<ILambdaContext>();
                Context.SetupGet(x => x.Logger).Returns(new Mock<ILambdaLogger>().Object);
            };

            It should_handle_CommandValidationException = async () =>
            {
                var commandName = "FakeCommand";
                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new CommandValidationException("invalid"));

                var result = await Subject.Handle(commandName, Request, Context.Object);

                result.ShouldBeError("invalid");
            };

            It should_handle_Exception = async () =>
            {
                var commandName = "FakeCommand";
                The<ICommandProcessor>().WhenToldTo(x => x.ProcessAsync(commandName, Moq.It.IsAny<string>())).Throw(new Exception("fail"));

                var result = await Subject.Handle(commandName, Request, Context.Object);

                result.StatusCode.ShouldEqual(500);
                result.ShouldBeError("fail");
            };

            static APIGatewayProxyRequest Request;
            static Mock<ILambdaContext> Context;
        }
    }
}
#endif