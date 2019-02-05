using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using Newtonsoft.Json.Linq;

namespace CommandQuery
{
    /// <summary>
    /// Process commands by invoking the corresponding handler.
    /// </summary>
    public interface ICommandProcessor
    {
        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ProcessAsync(string commandName, string json);

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ProcessAsync(string commandName, JObject json);

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ProcessAsync(ICommand command);

        /// <summary>
        /// Returns the types of commands that can be processed.
        /// </summary>
        /// <returns>Supported commands</returns>
        IEnumerable<Type> GetCommands();
    }

    /// <summary>
    /// Process commands by invoking the corresponding handler.
    /// </summary>
    public class CommandProcessor : ICommandProcessor
    {
        private readonly ITypeCollection _typeCollection;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProcessor" /> class.
        /// </summary>
        /// <param name="typeCollection">A collection of supported commands</param>
        /// <param name="serviceProvider">A service provider with supported command handlers</param>
        public CommandProcessor(ICommandTypeCollection typeCollection, IServiceProvider serviceProvider)
        {
            _typeCollection = typeCollection;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ProcessAsync(string commandName, string json)
        {
            await ProcessAsync(commandName, JObject.Parse(json));
        }

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ProcessAsync(string commandName, JObject json)
        {
            var commandType = _typeCollection.GetType(commandName);

            if (commandType == null) throw new CommandProcessorException($"The command type '{commandName}' could not be found");

            var command = json.SafeToObject(commandType);

            if (command == null) throw new CommandProcessorException("The json could not be converted to an object");

            await ProcessAsync((dynamic)command);
        }

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ProcessAsync(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic handler = _serviceProvider.GetService(handlerType);

            if (handler == null) throw new CommandProcessorException($"The command handler for '{command}' could not be found");

            await handler.HandleAsync((dynamic)command);
        }

        /// <summary>
        /// Returns the types of commands that can be processed.
        /// </summary>
        /// <returns>Supported commands</returns>
        public IEnumerable<Type> GetCommands()
        {
            return _typeCollection.GetTypes();
        }
    }
}