#if NET461
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.AspNet.WebApi;
using CommandQuery.Exceptions;
using Machine.Specifications;
using Moq;
using Newtonsoft.Json.Linq;
using ExceptionResult = System.Web.Http.Results.ExceptionResult;
using It = Machine.Specifications.It;

namespace CommandQuery.Specs.AspNet.WebApi
{
    public class BaseQueryControllerSpecs
    {
        [Subject(typeof(BaseQueryController))]
        public class when_handling_the_query
        {
            Establish context = () =>
            {
                FakeQueryProcessor = new Mock<IQueryProcessor>();

                Subject = new FakeQueryController(FakeQueryProcessor.Object)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };
            };

            public class method_Post
            {
                It should_invoke_the_query_processor = () =>
                {
                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");

                    Subject.HandlePost(queryName, json).Await();

                    FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(queryName, json));
                };

                It should_return_the_result_from_the_query_processor = () =>
                {
                    var expected = new object();
                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Returns(Task.FromResult(expected));

                    var result = Subject.HandlePost(queryName, json).Result as OkNegotiatedContentResult<object>;

                    result.Content.ShouldEqual(expected);
                };

                It should_handle_QueryValidationException = () =>
                {
                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Throws(new QueryValidationException("invalid"));

                    var result = Subject.HandlePost(queryName, json).Result as BadRequestErrorMessageResult;

                    result.ShouldBeError("invalid");
                };

                It should_handle_Exception = () =>
                {
                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Throws(new Exception("fail"));

                    var result = Subject.HandlePost(queryName, json).Result as ExceptionResult;

                    result.ShouldBeError("fail");
                };
            }

            public class method_Get
            {
                It should_invoke_the_query_processor = () =>
                {
                    var queryName = "FakeQuery";

                    Subject.HandleGet(queryName).Await();

                    FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<Dictionary<string, string>>()));
                };

                It should_return_the_result_from_the_query_processor = () =>
                {
                    var expected = new object();
                    var queryName = "FakeQuery";
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<Dictionary<string, string>>())).Returns(Task.FromResult(expected));

                    var result = Subject.HandleGet(queryName).Result as OkNegotiatedContentResult<object>;

                    result.Content.ShouldEqual(expected);
                };

                It should_handle_QueryValidationException = () =>
                {
                    var queryName = "FakeQuery";
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<Dictionary<string, string>>())).Throws(new QueryValidationException("invalid"));

                    var result = Subject.HandleGet(queryName).Result as BadRequestErrorMessageResult;

                    result.ShouldBeError("invalid");
                };

                It should_handle_Exception = () =>
                {
                    var queryName = "FakeQuery";
                    FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, Moq.It.IsAny<Dictionary<string, string>>())).Throws(new Exception("fail"));

                    var result = Subject.HandleGet(queryName).Result as ExceptionResult;

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

                Subject = new FakeQueryController(new QueryProcessor(new QueryTypeCollection(typeof(FakeQuery).Assembly), mock.Object))
                {
                    Request = new HttpRequestMessage(),
                    Configuration = new HttpConfiguration()
                };
            };

            public class method_Post
            {
                It should_work = () =>
                {
                    var expected = new FakeResult();

                    FakeQueryHandler.Setup(x => x.HandleAsync(Moq.It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                    var queryName = "FakeQuery";
                    var json = JObject.Parse("{}");

                    var result = Subject.HandlePost(queryName, json).Result as OkNegotiatedContentResult<object>;

                    result.Content.ShouldEqual(expected);
                };

                It should_handle_errors = () =>
                {
                    var queryName = "NotFoundQuery";
                    var json = JObject.Parse("{}");

                    var result = Subject.HandlePost(queryName, json).Result as BadRequestErrorMessageResult;

                    result.ShouldBeError("The query type 'NotFoundQuery' could not be found");
                };
            }

            public class method_Get
            {
                It should_work = () =>
                {
                    var expected = new FakeResult();

                    FakeQueryHandler.Setup(x => x.HandleAsync(Moq.It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                    var queryName = "FakeQuery";

                    var result = Subject.HandleGet(queryName).Result as OkNegotiatedContentResult<object>;

                    result.Content.ShouldEqual(expected);
                };

                It should_handle_errors = () =>
                {
                    var queryName = "NotFoundQuery";

                    var result = Subject.HandleGet(queryName).Result as BadRequestErrorMessageResult;

                    result.ShouldBeError("The query type 'NotFoundQuery' could not be found");
                };
            }

            static Mock<IQueryHandler<FakeQuery, FakeResult>> FakeQueryHandler;
            static BaseQueryController Subject;
        }
    }
}
#endif