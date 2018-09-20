using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.AspNet.WebApi
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceProvider(this IServiceCollection services)
        {
            services.AddTransient<IServiceProvider>(_ => services.BuildServiceProvider());

            return services;
        }

        public static IServiceCollection AddControllers(this IServiceCollection services, Assembly assembly = null)
        {
            var controllerTypes = (assembly ?? Assembly.GetCallingAssembly()).GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(ApiController).IsAssignableFrom(t) || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase));
            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }

            return services;
        }

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
            var handlers = assemblies.SelectMany(assembly => GetHandlers(assembly, genericType));

            foreach (var handler in handlers)
            {
                services.AddTransient(handler.GetHandlerInterface(genericType), handler);
            }
        }

        private static IEnumerable<Type> GetHandlers(Assembly assembly, Type genericType)
        {
            return assembly.GetTypes().Where(type => type.GetTypeInfo().IsClass && IsAssignableToGenericType(type, genericType));
        }

        private static bool IsAssignableToGenericType(Type type, Type genericType)
        {
            return type.GetInterfaces().Any(it => it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType)
                   || (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType)
                   || (type.GetTypeInfo().BaseType != null && IsAssignableToGenericType(type.GetTypeInfo().BaseType, genericType));
        }

        private static Type GetHandlerInterface(this Type type, Type genericType)
        {
            return type.GetInterfaces().FirstOrDefault(it => it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType);
        }
    }
}