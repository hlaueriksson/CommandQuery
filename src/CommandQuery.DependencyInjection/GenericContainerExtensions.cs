using System;
using System.Linq;
using System.Reflection;

namespace CommandQuery.DependencyInjection
{
    public static class GenericContainerExtensions
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
            var handlers = assemblies.SelectMany(assembly => assembly.GetHandlers(genericType));

            foreach (var handler in handlers)
            {
                registerType(handler.GetHandlerInterface(genericType), handler);
            }
        }
    }
}