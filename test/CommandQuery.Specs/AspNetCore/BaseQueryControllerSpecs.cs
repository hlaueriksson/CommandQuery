using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CommandQuery.AspNetCore;
using CommandQuery.Exceptions;
using Machine.Specifications;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using It = Machine.Specifications.It;

namespace CommandQuery.Specs.AspNetCore
{
    public class BaseQueryControllerSpecs
    {
        [Subject(typeof(BaseQueryController))]
        public class when_handling_the_query
        {
            private Establish context = () =>
            {
                FakeQueryProcessor = new Mock<IQueryProcessor>();
                
                Subject = new FakeQueryController(FakeQueryProcessor.Object)
                {
                    ControllerContext = Fake.ControllerContext()
                };
            };

            public class method_Post
            {
                It should_invoke_the_query_processor = async () =>
                {
                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");

                    await Subject.HandlePost(queryName, json);

                    FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(queryName, json));
                };

                It should_return_the_result_from_the_query_processor = async () =>
                {
                    var expected = new object();
                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Returns(Task.FromResult(expected));

                    var result = await Subject.HandlePost(queryName, json) as OkObjectResult;

                    result.Value.ShouldEqual(expected);
                };

                private It should_handle_QueryValidationException = async () =>
                {
                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Throws(new QueryValidationException("invalid"));

                    var result = await Subject.HandlePost(queryName, json) as BadRequestObjectResult;

                    result.ShouldBeError("invalid");
                };

                It should_handle_Exception = async () =>
                {
                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Throws(new Exception("fail"));

                    var result = await Subject.HandlePost(queryName, json) as ObjectResult;

                    result.StatusCode.ShouldEqual(500);
                    result.ShouldBeError("fail");
                };
            }

            public class method_Get
            {
                It should_invoke_the_query_processor = async () =>
                {
                    var queryName = "FakeQuery";

                    await Subject.HandleGet(queryName);

                    FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<Dictionary<string, string>>()));
                };

                It should_return_the_result_from_the_query_processor = async () =>
                {
                    var expected = new object();
                    var queryName = "FakeQuery";
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<Dictionary<string, string>>())).Returns(Task.FromResult(expected));

                    var result = await Subject.HandleGet(queryName) as OkObjectResult;

                    result.Value.ShouldEqual(expected);
                };

                private It should_handle_QueryValidationException = async () =>
                {
                    var queryName = "FakeQuery";
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<Dictionary<string, string>>())).Throws(new QueryValidationException("invalid"));

                    var result = await Subject.HandleGet(queryName) as BadRequestObjectResult;

                    result.ShouldBeError("invalid");
                };

                It should_handle_Exception = async () =>
                {
                    var queryName = "FakeQuery";
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<Dictionary<string, string>>())).Throws(new Exception("fail"));

                    var result = await Subject.HandleGet(queryName) as ObjectResult;

                    result.StatusCode.ShouldEqual(500);
                    result.ShouldBeError("fail");
                };
            }

            static Mock<IQueryProcessor> FakeQueryProcessor;
            static BaseQueryController Subject;
        }

        [Subject(typeof(BaseQueryController))]
        public class when_using_the_real_QueryProcessor
        {
            Establish context = () =>
            {
                FakeQueryHandler = new Mock<IQueryHandler<FakeQuery, FakeResult>>();

                var mock = new Mock<IServiceProvider>();
                mock.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(FakeQueryHandler.Object);

                Subject = new FakeQueryController(new QueryProcessor(new QueryTypeCollection(typeof(FakeQuery).GetTypeInfo().Assembly), mock.Object))
                {
                    ControllerContext = Fake.ControllerContext()
                };
            };

            public class method_Post
            {
                It should_work = async () =>
                {
                    var expected = new FakeResult();

                    FakeQueryHandler.Setup(x => x.HandleAsync(Moq.It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");

                    var result = await Subject.HandlePost(queryName, json) as OkObjectResult;

                    result.Value.ShouldEqual(expected);
                };

                It should_handle_errors = async () =>
                {
                    var queryName = "NotFoundQuery";
                    var json = JObject.Parse("{}");

                    var result = await Subject.HandlePost(queryName, json) as BadRequestObjectResult;

                    result.ShouldBeError("The query type 'NotFoundQuery' could not be found");
                };
            }

            public class method_Get
            {
                It should_work = async () =>
                {
                    var expected = new FakeResult();

                    FakeQueryHandler.Setup(x => x.HandleAsync(Moq.It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                    var queryName = "FakeQuery";

                    var result = await Subject.HandleGet(queryName) as OkObjectResult;

                    result.Value.ShouldEqual(expected);
                };

                It should_handle_errors = async () =>
                {
                    var queryName = "NotFoundQuery";

                    var result = await Subject.HandleGet(queryName) as BadRequestObjectResult;

                    result.ShouldBeError("The query type 'NotFoundQuery' could not be found");
                };
            }

            static Mock<IQueryHandler<FakeQuery, FakeResult>> FakeQueryHandler;
            static BaseQueryController Subject;
        }
    }
}