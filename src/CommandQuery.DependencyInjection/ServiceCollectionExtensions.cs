using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services, params Assembly[] assemblies)
        {
            var genericType = typeof(ICommandHandler<>);

            services.AddTransient<ICommandProcessor, CommandProcessor>();
            services.AddTransient<ICommandTypeCollection>(provider => new CommandTypeCollection(assemblies));

            services.AddHandlers(genericType, assemblies);

            return services;
        }

        public static IServiceCollection AddQueries(this IServiceCollection services, params Assembly[] assemblies)
        {
            var genericType = typeof(IQueryHandler<,>);

            services.AddTransient<IQueryProcessor, QueryProcessor>();
            services.AddTransient<IQueryTypeCollection>(provider => new QueryTypeCollection(assemblies));

            services.AddHandlers(genericType, assemblies);

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