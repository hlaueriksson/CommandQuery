using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity;

namespace CommandQuery.Sample.AspNet.WebApi
{
    public static class ContainerExtensions
    {
        public static IUnityContainer RegisterCommands(this IUnityContainer container, params Assembly[] assemblies)
        {
            var genericType = typeof(ICommandHandler<>);

            container.RegisterType<ICommandProcessor, CommandProcessor>();
            container.RegisterInstance<ICommandTypeCollection>(new CommandTypeCollection(assemblies));

            container.RegisterHandlers(genericType, assemblies);

            return container;
        }

        public static IUnityContainer RegisterQueries(this IUnityContainer container, params Assembly[] assemblies)
        {
            var genericType = typeof(IQueryHandler<,>);

            container.RegisterType<IQueryProcessor, QueryProcessor>();
            container.RegisterInstance<IQueryTypeCollection>(new QueryTypeCollection(assemblies));

            container.RegisterHandlers(genericType, assemblies);

            return container;
        }

        private static void RegisterHandlers(this IUnityContainer container, Type genericType, params Assembly[] assemblies)
        {
            var handlers = assemblies.SelectMany(assembly => GetHandlers(assembly, genericType));

            foreach (var handler in handlers)
            {
                container.RegisterType(handler.GetHandlerInterface(genericType), handler);
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

    public class UnityServiceProvider : IServiceProvider
    {
        private readonly IUnityContainer _container;

        public UnityServiceProvider(IUnityContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }
    }
}