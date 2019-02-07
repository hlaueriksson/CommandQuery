#if NET471
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using CommandQuery.AspNet.WebApi;
using CommandQuery.Exceptions;
using FluentAssertions;
using LoFuUnit.NUnit;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CommandQuery.Tests.AspNet.WebApi
{
    public class BaseQueryControllerTests
    {
        [LoFu, Test]
        public async Task when_handling_the_query_via_Post()
        {
            FakeQueryProcessor = new Mock<IQueryProcessor>();

            Subject = new FakeQueryController(FakeQueryProcessor.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            async Task should_invoke_the_query_processor()
            {
                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");

                await Subject.HandlePost(queryName, json);

                FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(queryName, json));
            }

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new object();
                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Returns(Task.FromResult(expected));

                var result = await Subject.HandlePost(queryName, json) as OkNegotiatedContentResult<object>;

                result.Content.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandlePost(queryName, json) as BadRequestErrorMessageResult;

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, json)).Throws(new Exception("fail"));

                var result = await Subject.HandlePost(queryName, json) as ExceptionResult;

                await result.ShouldBeErrorAsync("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get()
        {
            FakeQueryProcessor = new Mock<IQueryProcessor>();

            Subject = new FakeQueryController(FakeQueryProcessor.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            async Task should_invoke_the_query_processor()
            {
                var queryName = "FakeQuery";

                await Subject.HandleGet(queryName);

                FakeQueryProcessor.Verify(x => x.ProcessAsync<object>(queryName, It.IsAny<Dictionary<string, string>>()));
            }

            async Task should_return_the_result_from_the_query_processor()
            {
                var expected = new object();
                var queryName = "FakeQuery";
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, It.IsAny<Dictionary<string, string>>())).Returns(Task.FromResult(expected));

                var result = await Subject.HandleGet(queryName) as OkNegotiatedContentResult<object>;

                result.Content.Should().Be(expected);
            }

            async Task should_handle_QueryValidationException()
            {
                var queryName = "FakeQuery";
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, It.IsAny<Dictionary<string, string>>())).Throws(new QueryValidationException("invalid"));

                var result = await Subject.HandleGet(queryName) as BadRequestErrorMessageResult;

                await result.ShouldBeErrorAsync("invalid");
            }

            async Task should_handle_Exception()
            {
                var queryName = "FakeQuery";
                FakeQueryProcessor.Setup(x => x.ProcessAsync<object>(queryName, It.IsAny<Dictionary<string, string>>())).Throws(new Exception("fail"));

                var result = await Subject.HandleGet(queryName) as ExceptionResult;

                await result.ShouldBeErrorAsync("fail");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Post_with_real_QueryProcessor()
        {
            FakeQueryHandler = new Mock<IQueryHandler<FakeQuery, FakeResult>>();

            var mock = new Mock<IServiceProvider>();
            mock.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(FakeQueryHandler.Object);

            Subject = new FakeQueryController(new QueryProcessor(new QueryTypeCollection(typeof(FakeQuery).Assembly), mock.Object))
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            async Task should_work()
            {
                var expected = new FakeResult();

                FakeQueryHandler.Setup(x => x.HandleAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var queryName = "FakeQuery";
                var json = JObject.Parse("{}");

                var result = await Subject.HandlePost(queryName, json) as OkNegotiatedContentResult<object>;

                result.Content.Should().Be(expected);
            }

            async Task should_handle_errors()
            {
                var queryName = "NotFoundQuery";
                var json = JObject.Parse("{}");

                var result = await Subject.HandlePost(queryName, json) as BadRequestErrorMessageResult;

                await result.ShouldBeErrorAsync("The query type 'NotFoundQuery' could not be found");
            }
        }

        [LoFu, Test]
        public async Task when_handling_the_query_via_Get_with_real_QueryProcessor()
        {
            FakeQueryHandler = new Mock<IQueryHandler<FakeQuery, FakeResult>>();

            var mock = new Mock<IServiceProvider>();
            mock.Setup(x => x.GetService(typeof(IQueryHandler<FakeQuery, FakeResult>))).Returns(FakeQueryHandler.Object);

            Subject = new FakeQueryController(new QueryProcessor(new QueryTypeCollection(typeof(FakeQuery).Assembly), mock.Object))
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            async Task should_work()
            {
                var expected = new FakeResult();

                FakeQueryHandler.Setup(x => x.HandleAsync(It.IsAny<FakeQuery>())).Returns(Task.FromResult(expected));

                var queryName = "FakeQuery";

                var result = await Subject.HandleGet(queryName) as OkNegotiatedContentResult<object>;

                result.Content.Should().Be(expected);
            }

            async Task should_handle_errors()
            {
                var queryName = "NotFoundQuery";

                var result = await Subject.HandleGet(queryName) as BadRequestErrorMessageResult;

                await result.ShouldBeErrorAsync("The query type 'NotFoundQuery' could not be found");
            }
        }

        Mock<IQueryProcessor> FakeQueryProcessor;
        Mock<IQueryHandler<FakeQuery, FakeResult>> FakeQueryHandler;
        BaseQueryController Subject;
    }
}
#endif