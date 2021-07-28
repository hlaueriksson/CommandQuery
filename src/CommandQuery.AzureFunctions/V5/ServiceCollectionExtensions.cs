#if NET5_0
using System.Reflection;
using CommandQuery.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.AzureFunctions
{
    /// <summary>
    /// Extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds function and command handlers to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="assemblies">Assemblies with command handlers.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddCommandFunction(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton<ICommandFunction, CommandFunction>();
            services.AddCommands(assemblies);

            return services;
        }

        /// <summary>
        /// Adds function and query handlers to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="assemblies">Assemblies with query handlers.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddQueryFunction(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton<IQueryFunction, QueryFunction>();
            services.AddQueries(assemblies);

            return services;
        }
    }
}
#endif
