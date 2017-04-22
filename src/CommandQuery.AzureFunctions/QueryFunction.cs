using System;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json.Linq;
using System.Reflection;
using CommandQuery.AzureFunctions.Internal;

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