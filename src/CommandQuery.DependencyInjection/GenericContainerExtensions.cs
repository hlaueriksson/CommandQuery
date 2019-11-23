using System;
using System.Linq;
using System.Reflection;
using CommandQuery.DependencyInjection.Internal;

namespace CommandQuery.DependencyInjection
{
    /// <summary>
    /// Extensions methods for IoC Containers.
    /// </summary>
    public static class GenericContainerExtensions
    {
        /// <summary>
        /// Register command handlers to a custom IoC Container.
        /// </summary>
        /// <param name="assemblies">Assemblies with command handlers</param>
        /// <param name="registerType">A delegate to register types</param>
        /// <param name="registerInstance">A delegate to register instances</param>
        public static void RegisterCommands(this Assembly[] assemblies, Action<Type, Type> registerType, Action<Type, object> registerInstance)
        {
            registerType(typeof(ICommandProcessor), typeof(CommandProcessor));
            registerInstance(typeof(ICommandTypeCollection), new CommandTypeCollection(assemblies));

            assemblies.RegisterHandlers(typeof(ICommandHandler<>), registerType);
            assemblies.RegisterHandlers(typeof(ICommandHandler<,>), registerType);
        }

        /// <summary>
        /// Register query handlers to a custom IoC Container.
        /// </summary>
        /// <param name="assemblies">Assemblies with query handlers</param>
        /// <param name="registerType">A delegate to register types</param>
        /// <param name="registerInstance">A delegate to register instances</param>
        public static void RegisterQueries(this Assembly[] assemblies, Action<Type, Type> registerType, Action<Type, object> registerInstance)
        {
            registerType(typeof(IQueryProcessor), typeof(QueryProcessor));
            registerInstance(typeof(IQueryTypeCollection), new QueryTypeCollection(assemblies));

            assemblies.RegisterHandlers(typeof(IQueryHandler<,>), registerType);
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