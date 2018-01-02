using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using CommandQuery.AzureFunctions.Internal;
using CommandQuery.Exceptions;
using Newtonsoft.Json.Linq;

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

#if NET46
        public async Task<HttpResponseMessage> Handle(string queryName, HttpRequestMessage req, Microsoft.Azure.WebJobs.Host.TraceWriter log)
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