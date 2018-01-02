using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using CommandQuery.AzureFunctions.Internal;
using CommandQuery.Exceptions;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;

#if NET461
using System.Net;
using System.Net.Http;
#endif

#if NETSTANDARD2_0
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
#endif

namespace CommandQuery.AzureFunctions
{
    public class QueryFunction
    {
        private readonly IQueryProcessor _queryProcessor;

        public QueryFunction(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        public async Task<object> Handle(string queryName, string content)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, JObject.Parse(content));
        }

#if NET461
        public async Task<HttpResponseMessage> Handle(string queryName, HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Handle {queryName}");

            try
            {
                var result = await Handle(queryName, await req.Content.ReadAsStringAsync());

                return req.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (QueryProcessorException exception)
            {
                log.Error("Handle query failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (QueryValidationException exception)
            {
                log.Error("Handle query failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (Exception exception)
            {
                log.Error("Handle query failed", exception);

                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, exception.Message);
            }
        }
#endif

#if NETSTANDARD2_0
        public async Task<IActionResult> Handle(string queryName, HttpRequest req, TraceWriter log)
        {
            log.Info($"Handle {queryName}");

            try
            {
                var result = await Handle(queryName, await req.ReadAsStringAsync());

                return new OkObjectResult(result);
            }
            catch (QueryProcessorException exception)
            {
                log.Error("Handle query failed", exception);

                return new BadRequestObjectResult(exception.Message);
            }
            catch (QueryValidationException exception)
            {
                log.Error("Handle query failed", exception);

                return new BadRequestObjectResult(exception.Message);
            }
            catch (Exception exception)
            {
                log.Error("Handle query failed", exception);

                return new ObjectResult(exception.Message)
                {
                    StatusCode = 500
                };
            }
        }
#endif
    }

    public class QueryServiceProvider : IServiceProvider
    {
        private IContainer _container;

        public QueryServiceProvider(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }
    }

    public static class QueryExtensions
    {
        public static IQueryProcessor GetQueryProcessor(this Assembly assembly)
        {
            var builder = new ContainerBuilder();
            builder.AddQueries(assembly);

            return new QueryProcessor(new QueryTypeCollection(assembly), new QueryServiceProvider(builder.Build()));
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly[] assemblies)
        {
            var builder = new ContainerBuilder();
            builder.AddQueries(assemblies);

            return new QueryProcessor(new QueryTypeCollection(assemblies), new QueryServiceProvider(builder.Build()));
        }
    }
}