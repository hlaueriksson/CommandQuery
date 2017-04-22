using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

namespace CommandQuery.AzureFunctions.Internal
{
    internal static class ContainerBuilderExtensions
    {
        public static void AddCommands(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            var genericType = typeof(ICommandHandler<>);

            builder.RegisterType<CommandProcessor>().As<ICommandProcessor>();
            builder.RegisterInstance(new CommandTypeCollection(assemblies)).As<ICommandTypeCollection>();

            builder.AddHandlers(genericType, assemblies);
        }

        public static void AddQueries(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            var genericType = typeof(IQueryHandler<,>);

            builder.RegisterType<QueryProcessor>().As<IQueryProcessor>();
            builder.RegisterInstance(new QueryTypeCollection(assemblies)).As<IQueryTypeCollection>();

            builder.AddHandlers(genericType, assemblies);
        }

        private static void AddHandlers(this ContainerBuilder builder, Type genericType, params Assembly[] assemblies)
        {
            var handlers = assemblies.SelectMany(assembly => GetHandlers(assembly, genericType));

            foreach (var handler in handlers)
            {
                builder.RegisterType(handler).As(handler.GetHandlerInterface(genericType));
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