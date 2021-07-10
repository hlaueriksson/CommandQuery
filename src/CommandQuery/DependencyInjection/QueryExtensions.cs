using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.DependencyInjection
{
    /// <summary>
    /// Extensions methods for initializing an <see cref="IQueryProcessor"/>.
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Initializes an <see cref="IQueryProcessor"/> from an <see cref="Assembly"/> with queries and handlers.
        /// </summary>
        /// <param name="assembly">An <see cref="Assembly"/> with queries and handlers.</param>
        /// <returns>An <see cref="IQueryProcessor"/>.</returns>
        public static IQueryProcessor GetQueryProcessor(this Assembly assembly)
        {
            return new ServiceCollection().GetQueryProcessor(assembly);
        }

        /// <summary>
        /// Initializes an <see cref="IQueryProcessor"/> from assemblies with queries and handlers.
        /// </summary>
        /// <param name="assemblies">Assemblies with queries and handlers.</param>
        /// <returns>An <see cref="IQueryProcessor"/>.</returns>
        public static IQueryProcessor GetQueryProcessor(this Assembly[] assemblies)
        {
            return new ServiceCollection().GetQueryProcessor(assemblies);
        }

        /// <summary>
        /// Initializes an <see cref="IQueryProcessor"/> from an <see cref="Assembly"/> with queries and handlers.
        /// </summary>
        /// <param name="assembly">An <see cref="Assembly"/> with queries and handlers.</param>
        /// <param name="services">A service collection for query handlers.</param>
        /// <returns>An <see cref="IQueryProcessor"/>.</returns>
        public static IQueryProcessor GetQueryProcessor(this Assembly assembly, IServiceCollection services)
        {
            return services.GetQueryProcessor(assembly);
        }

        /// <summary>
        /// Initializes an <see cref="IQueryProcessor"/> from assemblies with queries and handlers.
        /// </summary>
        /// <param name="assemblies">Assemblies with queries and handlers.</param>
        /// <param name="services">A service collection for query handlers.</param>
        /// <returns>An <see cref="IQueryProcessor"/>.</returns>
        public static IQueryProcessor GetQueryProcessor(this Assembly[] assemblies, IServiceCollection services)
        {
            return services.GetQueryProcessor(assemblies);
        }

        /// <summary>
        /// Initializes an <see cref="IQueryProcessor"/> from assemblies with queries and handlers.
        /// </summary>
        /// <param name="services">A service collection for query handlers.</param>
        /// <param name="assemblies">Assemblies with queries and handlers.</param>
        /// <returns>An <see cref="IQueryProcessor"/>.</returns>
        public static IQueryProcessor GetQueryProcessor(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddQueries(assemblies);

            return new QueryProcessor(new QueryTypeProvider(assemblies), services.BuildServiceProvider());
        }
    }
}
