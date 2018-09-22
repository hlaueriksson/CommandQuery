using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.DependencyInjection
{
    public static class QueryExtensions
    {
        public static IQueryProcessor GetQueryProcessor(this Assembly assembly)
        {
            return GetQueryProcessor(new ServiceCollection(), assembly);
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly[] assemblies)
        {
            return GetQueryProcessor(new ServiceCollection(), assemblies);
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly assembly, IServiceCollection services)
        {
            return GetQueryProcessor(services, assembly);
        }

        public static IQueryProcessor GetQueryProcessor(this Assembly[] assemblies, IServiceCollection services)
        {
            return GetQueryProcessor(services, assemblies);
        }

        private static IQueryProcessor GetQueryProcessor(IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddQueries(assemblies);

            return new QueryProcessor(new QueryTypeCollection(assemblies), services.BuildServiceProvider());
        }
    }
}