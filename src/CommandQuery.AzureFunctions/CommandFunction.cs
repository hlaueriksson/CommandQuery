using System;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json.Linq;
using System.Reflection;
using CommandQuery.AzureFunctions.Internal;

namespace CommandQuery.AzureFunctions
{
    public class CommandFunction
    {
        private readonly ICommandProcessor _commandProcessor;

        public CommandFunction(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        public async Task Handle(string commandName, string content)
        {
            await _commandProcessor.ProcessAsync(commandName, JObject.Parse(content));
        }
    }

    public class CommandServiceProvider : IServiceProvider
    {
        private IContainer _container;

        public CommandServiceProvider(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }
    }

    public static class CommandExtensions
    {
        public static ICommandProcessor GetCommandProcessor(this Assembly assembly)
        {
            var builder = new ContainerBuilder();
            builder.AddCommands(assembly);

            return new CommandProcessor(new CommandTypeCollection(assembly), new CommandServiceProvider(builder.Build()));
        }

        public static ICommandProcessor GetCommandProcessor(this Assembly[] assemblies)
        {
            var builder = new ContainerBuilder();
            builder.AddCommands(assemblies);

            return new CommandProcessor(new CommandTypeCollection(assemblies), new CommandServiceProvider(builder.Build()));
        }
    }
}