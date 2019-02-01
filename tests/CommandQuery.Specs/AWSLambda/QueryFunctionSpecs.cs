#if NETCOREAPP2_0
using System;
using System.Collections.Generic;
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
    public class QueryFunctionSpecs
    {
        [Subject(typeof(QueryFunction))]
        public class when_handling_the_query : WithSubject<QueryFunction>
        {
            public class method_Post
            {
                It should_invoke_the_query_processor = () =>
                {
                    var queryName = "FakeQuery";
                    var content = "{}";

                    Subject.Handle(queryName, content).Await();

                    The<IQueryProcessor>().WasToldTo(x => x.ProcessAsync<object>(queryName, content));
                };

                It base_method_should_not_handle_Exception = () =>
                {
                    var queryName = "FakeQuery";
                    var content = "{}";

                    The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, content)).Throw(new Exception("fail"));

                    Catch.Exception(() => Subject.Handle(queryName, content).Await()).ShouldBeOfExactType<Exception>();
                };
            }

            public class method_Get
            {
                It should_invoke_the_query_processor = () =>
                {
                    var queryName = "FakeQuery";
                    var query = new Dictionary<string, string>();

                    Subject.Handle(queryName, query).Await();

                    The<IQueryProcessor>().WasToldTo(x => x.ProcessAsync<object>(queryName, query));
                };

                It base_method_should_not_handle_Exception = () =>
                {
                    var queryName = "FakeQuery";
                    var query = new Dictionary<string, string>();

                    The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, query)).Throw(new Exception("fail"));

                    Catch.Exception(() => Subject.Handle(queryName, query).Await()).ShouldBeOfExactType<Exception>();
                };
            }

            public class method_lambda_context
            {
                Establish context = () =>
                {
                    Request = new APIGatewayProxyRequest();
                    Context = new Mock<ILambdaContext>();
                    Context.SetupGet(x => x.Logger).Returns(new Mock<ILambdaLogger>().Object);
                };

                It should_handle_QueryValidationException = async () =>
                {
                    var queryName = "FakeQuery";
                    The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<string>())).Throw(new QueryValidationException("invalid"));

                    var result = await Subject.Handle(queryName, Request, Context.Object);

                    result.ShouldBeError("invalid");
                };

                It should_handle_Exception = async () =>
                {
                    var queryName = "FakeQuery";
                    The<IQueryProcessor>().WhenToldTo(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<string>())).Throw(new Exception("fail"));

                    var result = await Subject.Handle(queryName, Request, Context.Object);

                    result.StatusCode.ShouldEqual(500);
                    result.ShouldBeError("fail");
                };

                static APIGatewayProxyRequest Request;
                static Mock<ILambdaContext> Context;
            }
        }
    }
}
#endif