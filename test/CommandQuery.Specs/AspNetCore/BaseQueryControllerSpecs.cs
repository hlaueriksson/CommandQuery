using System;
using System.Reflection;
using System.Threading.Tasks;
using CommandQuery.AspNetCore;
using CommandQuery.Exceptions;
using Machine.Specifications;
using Microsoft.AspNetCore.Http;
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
            Establish context = () =>
            {
                FakeQueryProcessor = new Mock<IQueryProcessor>();
                FakeHttpResponse = new Mock<HttpResponse>();

                Subject = new FakeQueryController(FakeQueryProcessor.Object)
                {
                    ControllerContext = Fake.ControllerContext(fakeHttpResponse: FakeHttpResponse)
                };
            };

            It should_invoke_the_query_processor = async () =>
            {
                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");

                await Subject.Handle(queryName, json);

                FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(queryName, json));
            };

            It should_return_the_result_from_the_query_processor = async () =>
            {
                var expected = new object();
                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Returns(Task.FromResult(expected));

                var result = await Subject.Handle(queryName, json);

                result.ShouldEqual(expected);
            };

            private It should_handle_QueryValidationException = async () =>
            {
                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Throws(new QueryValidationException("invalid"));

                var result = await Subject.Handle(queryName, json) as BadRequestObjectResult;

                result.Value.ShouldEqual("invalid");
            };

            It should_handle_Exception = async () =>
            {
                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Throws(new Exception("fail"));

                var result = await Subject.Handle(queryName, json) as ObjectResult;

                result.StatusCode.ShouldEqual(500);
                result.Value.ShouldEqual("fail");
            };

            static Mock<IQueryProcessor> FakeQueryProcessor;
            static Mock<HttpResponse> FakeHttpResponse;
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

            It should_work = async () =>
            {
                var expected = new FakeResult();

                FakeQueryHandler.Setup(x => x.HandleAsync(Moq.It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");

                var result = await Subject.Handle(queryName, json) as FakeResult;

                result.ShouldEqual(expected);
            };

            It should_handle_errors = async () =>
            {
                var queryName = "NotFoundQuery";
                var json = JObject.Parse("{}");

                var result = await Subject.Handle(queryName, json) as BadRequestObjectResult;

                result.Value.ShouldEqual("The query type 'NotFoundQuery' could not be found");
            };

            static Mock<IQueryHandler<FakeQuery, FakeResult>> FakeQueryHandler;
            static BaseQueryController Subject;
        }
    }
}