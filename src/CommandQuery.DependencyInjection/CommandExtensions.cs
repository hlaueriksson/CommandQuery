using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.DependencyInjection
{
    /// <summary>
    /// Extensions methods for initializing an <see cref="ICommandProcessor"/>.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Initializes an <see cref="ICommandProcessor"/> from an <see cref="Assembly"/> with commands and handlers.
        /// </summary>
        /// <param name="assembly">An <see cref="Assembly"/> with commands and handlers</param>
        /// <returns>An <see cref="ICommandProcessor"/></returns>
        public static ICommandProcessor GetCommandProcessor(this Assembly assembly)
        {
            return GetCommandProcessor(new ServiceCollection(), assembly);
        }

        /// <summary>
        /// Initializes an <see cref="ICommandProcessor"/> from assemblies with commands and handlers.
        /// </summary>
        /// <param name="assemblies">Assemblies with commands and handlers</param>
        /// <returns>An <see cref="ICommandProcessor"/></returns>
        public static ICommandProcessor GetCommandProcessor(this Assembly[] assemblies)
        {
            return GetCommandProcessor(new ServiceCollection(), assemblies);
        }

        /// <summary>
        /// Initializes an <see cref="ICommandProcessor"/> from an <see cref="Assembly"/> with commands and handlers.
        /// </summary>
        /// <param name="assembly">An <see cref="Assembly"/> with commands and handlers</param>
        /// <param name="services">A service collection for command handlers</param>
        /// <returns>An <see cref="ICommandProcessor"/></returns>
        public static ICommandProcessor GetCommandProcessor(this Assembly assembly, IServiceCollection services)
        {
            return GetCommandProcessor(services, assembly);
        }

        /// <summary>
        /// Initializes an <see cref="ICommandProcessor"/> from assemblies with commands and handlers.
        /// </summary>
        /// <param name="assemblies">Assemblies with commands and handlers</param>
        /// <param name="services">A service collection for command handlers</param>
        /// <returns>An <see cref="ICommandProcessor"/></returns>
        public static ICommandProcessor GetCommandProcessor(this Assembly[] assemblies, IServiceCollection services)
        {
            return GetCommandProcessor(services, assemblies);
        }

        private static ICommandProcessor GetCommandProcessor(IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddCommands(assemblies);

            return new CommandProcessor(new CommandTypeCollection(assemblies), services.BuildServiceProvider());
        }
    }
}