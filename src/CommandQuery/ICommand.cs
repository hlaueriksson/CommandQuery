using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Internal;
using Newtonsoft.Json.Linq;

namespace CommandQuery
{
    public interface ICommand
    {
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }

    public interface ICommandProcessor
    {
        Task ProcessAsync(string commandName, JObject json);

        Task ProcessAsync(ICommand command);

        IEnumerable<Type> GetCommands();
    }

    public class CommandProcessor : ICommandProcessor
    {
        private readonly ITypeCollection _typeCollection;
        private readonly IServiceProvider _serviceProvider;

        public CommandProcessor(ICommandTypeCollection typeCollection, IServiceProvider serviceProvider)
        {
            _typeCollection = typeCollection;
            _serviceProvider = serviceProvider;
        }

        public async Task ProcessAsync(string commandName, JObject json)
        {
            var commandType = _typeCollection.GetType(commandName);

            if (commandType == null) throw new CommandProcessorException($"The command type '{commandName}' could not be found");

            var command = json.SafeToObject(commandType);

            if (command == null) throw new CommandProcessorException("The json could not be converted to an object");

            await ProcessAsync((dynamic)command);
        }

        public async Task ProcessAsync(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic handler = _serviceProvider.GetService(handlerType);

            if (handler == null) throw new CommandProcessorException($"The command handler for '{command}' could not be found");

            await handler.HandleAsync((dynamic)command);
        }

        public IEnumerable<Type> GetCommands()
        {
            return _typeCollection.GetTypes();
        }
    }
}