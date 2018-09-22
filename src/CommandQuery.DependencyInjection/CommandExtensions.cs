using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.DependencyInjection
{
    public static class CommandExtensions
    {
        public static ICommandProcessor GetCommandProcessor(this Assembly assembly)
        {
            return GetCommandProcessor(new ServiceCollection(), assembly);
        }

        public static ICommandProcessor GetCommandProcessor(this Assembly[] assemblies)
        {
            return GetCommandProcessor(new ServiceCollection(), assemblies);
        }

        public static ICommandProcessor GetCommandProcessor(this Assembly assembly, IServiceCollection services)
        {
            return GetCommandProcessor(services, assembly);
        }

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