using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.DependencyInjection
{
    /// <summary>
    /// Extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds command handlers to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/></param>
        /// <param name="assemblies">Assemblies with command handlers</param>
        /// <returns>The <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddCommands(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddTransient<ICommandProcessor, CommandProcessor>();
            services.AddTransient<ICommandTypeCollection>(provider => new CommandTypeCollection(assemblies));

            services.AddHandlers(typeof(ICommandHandler<>), assemblies);
            services.AddHandlers(typeof(ICommandHandler<,>), assemblies);

            return services;
        }

        /// <summary>
        /// Adds query handlers to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/></param>
        /// <param name="assemblies">Assemblies with query handlers</param>
        /// <returns>The <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddQueries(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddTransient<IQueryProcessor, QueryProcessor>();
            services.AddTransient<IQueryTypeCollection>(provider => new QueryTypeCollection(assemblies));

            services.AddHandlers(typeof(IQueryHandler<,>), assemblies);

            return services;
        }

        private static void AddHandlers(this IServiceCollection services, Type genericType, params Assembly[] assemblies)
        {
            var handlers = assemblies.SelectMany(assembly => assembly.GetHandlers(genericType));

            foreach (var handler in handlers)
            {
                services.AddTransient(handler.GetHandlerInterface(genericType), handler);
            }
        }
    }
}