#if !NETSTANDARD
using System.Reflection;
using CommandQuery.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// Extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds controllers and command handlers to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="assemblies">Assemblies with command handlers.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddCommandControllers(this IServiceCollection services, params Assembly[] assemblies)
        {
            services
                .AddControllers(options => options.Conventions.Add(new CommandQueryControllerModelConvention()))
                .ConfigureApplicationPartManager(manager => manager.FeatureProviders.Add(new CommandControllerFeatureProvider(assemblies)));
            services.AddCommands(assemblies);

            return services;
        }

        /// <summary>
        /// Adds controllers and query handlers to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="assemblies">Assemblies with query handlers.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddQueryControllers(this IServiceCollection services, params Assembly[] assemblies)
        {
            services
                .AddControllers(options => options.Conventions.Add(new CommandQueryControllerModelConvention()))
                .ConfigureApplicationPartManager(manager => manager.FeatureProviders.Add(new QueryControllerFeatureProvider(assemblies)));
            services.AddQueries(assemblies);

            return services;
        }
    }
}
#endif
