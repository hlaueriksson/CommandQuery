using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandQuery.AspNet.WebApi
{
    public static class ContainerExtensions
    {
        public static void RegisterCommands(this Assembly[] assemblies, Action<Type, Type> registerType, Action<Type, object> registerInstance)
        {
            var genericType = typeof(ICommandHandler<>);

            registerType(typeof(ICommandProcessor), typeof(CommandProcessor));
            registerInstance(typeof(ICommandTypeCollection), new CommandTypeCollection(assemblies));

            assemblies.RegisterHandlers(genericType, registerType);
        }

        public static void RegisterQueries(this Assembly[] assemblies, Action<Type, Type> registerType, Action<Type, object> registerInstance)
        {
            var genericType = typeof(IQueryHandler<,>);

            registerType(typeof(IQueryProcessor), typeof(QueryProcessor));
            registerInstance(typeof(IQueryTypeCollection), new QueryTypeCollection(assemblies));

            assemblies.RegisterHandlers(genericType, registerType);
        }

        private static void RegisterHandlers(this Assembly[] assemblies, Type genericType, Action<Type, Type> registerType)
        {
            var handlers = assemblies.SelectMany(assembly => GetHandlers(assembly, genericType));

            foreach (var handler in handlers)
            {
                registerType(handler.GetHandlerInterface(genericType), handler);
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