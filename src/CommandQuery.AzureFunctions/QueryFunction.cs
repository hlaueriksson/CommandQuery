using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using CommandQuery.AzureFunctions.Internal;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using Microsoft.Azure.WebJobs.Host;

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
            return await _queryProcessor.ProcessAsync<object>(queryName, content);
        }

        public async Task<object> Handle(string queryName, IDictionary<string, string> query)
        {
            return await _queryProcessor.ProcessAsync<object>(queryName, query);
        }

#if NET461
        public async Task<HttpResponseMessage> Handle(string queryName, HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Handle {queryName}");

            try
            {
                var result = req.Method == HttpMethod.Get
                    ? await Handle(queryName, req.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase))
                    : await Handle(queryName, await req.Content.ReadAsStringAsync());

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
                var result = req.Method == "GET"
                    ? await Handle(queryName, req.GetQueryParameterDictionary())
                    : await Handle(queryName, await req.ReadAsStringAsync());

                return new OkObjectResult(result);
            }
            catch (QueryProcessorException exception)
            {
                log.Error("Handle query failed", exception);

                return new BadRequestObjectResult(exception.ToError());
            }
            catch (QueryValidationException exception)
            {
                log.Error("Handle query failed", exception);

                return new BadRequestObjectResult(exception.ToError());
            }
            catch (Exception exception)
            {
                log.Error("Handle query failed", exception);

                return new ObjectResult(exception.ToError())
                {
                    StatusCode = 500
                };
            }
        }
#endif
    }

    public class QueryServiceProvider : IServiceProvider
    {
        private readonly IContainer _container;

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
            return GetQueryProcessor(new ContainerBuilder(), assembly);
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly[] assemblies)
        {
            return GetQueryProcessor(new ContainerBuilder(), assemblies);
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly assembly, ContainerBuilder containerBuilder)
        {
            return GetQueryProcessor(containerBuilder, assembly);
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly[] assemblies, ContainerBuilder containerBuilder)
        {
            return GetQueryProcessor(containerBuilder, assemblies);
        }

        private static IQueryProcessor GetQueryProcessor(ContainerBuilder containerBuilder, params Assembly[] assemblies)
        {
            containerBuilder.AddQueries(assemblies);

            return new QueryProcessor(new QueryTypeCollection(assemblies), new QueryServiceProvider(containerBuilder.Build()));
        }
    }
}