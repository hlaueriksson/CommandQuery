using System;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using CommandQuery.Sample.AspNet.WebApi.Controllers;
using CommandQuery.Sample.Queries;
using Machine.Specifications;
using Newtonsoft.Json.Linq;

namespace CommandQuery.Sample.AspNet.WebApi.Specs.Controllers
{
    public class QueryControllerSpecs
    {
        [Subject(typeof(QueryController))]
        public class when_using_the_real_controller
        {
            Establish context = () =>
            {
                var configuration = new HttpConfiguration();
                WebApiConfig.Register(configuration);

                var queryProcessor = configuration.DependencyResolver.GetService(typeof(IQueryProcessor)) as IQueryProcessor;

                Subject = new QueryController(queryProcessor)
                {
                    Request = new HttpRequestMessage(),
                    Configuration = configuration
                };
            };

            public class method_Post
            {
                It should_work = () =>
                {
                    var json = JObject.Parse("{ 'Id': 1 }");
                    var result = Subject.HandlePost("BarQuery", json).Result.ExecuteAsync(CancellationToken.None).Result;
                    var value = result.Content.ReadAsAsync<Bar>().Result;

                    result.EnsureSuccessStatusCode();
                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var json = JObject.Parse("{ 'Id': 1 }");
                    var result = Subject.HandlePost("FailQuery", json).Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            public class method_Get
            {
                Establish context = () =>
                {
                    Subject.Request = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://localhost/api/query/BarQuery?Id=1")
                    };
                };

                It should_work = () =>
                {
                    var result = Subject.HandleGet("BarQuery").Result.ExecuteAsync(CancellationToken.None).Result;
                    var value = result.Content.ReadAsAsync<Bar>().Result;

                    result.EnsureSuccessStatusCode();
                    value.Id.ShouldEqual(1);
                    value.Value.ShouldNotBeEmpty();
                };

                It should_handle_errors = () =>
                {
                    var result = Subject.HandleGet("FailQuery").Result;

                    result.ShouldBeError("The query type 'FailQuery' could not be found");
                };
            }

            static QueryController Subject;
        }
    }
}