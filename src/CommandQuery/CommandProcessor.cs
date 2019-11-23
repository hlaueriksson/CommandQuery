using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;

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
        /// <param name="command">The command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ProcessAsync(ICommand command);

        /// <summary>
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="command">The command</param>
        /// <returns>The result of the command</returns>
        Task<TResult> ProcessWithResultAsync<TResult>(ICommand<TResult> command);

        /// <summary>
        /// Returns the types of commands that can be processed.
        /// </summary>
        /// <returns>Supported commands</returns>
        IEnumerable<Type> GetCommandTypes();

        Type GetCommandType(string commandName);
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
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="command">The command</param>
        /// <returns>The result of the command</returns>
        public async Task<TResult> ProcessWithResultAsync<TResult>(ICommand<TResult> command)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetService(handlerType);

            if (handler == null) throw new CommandProcessorException($"The command handler for '{command}' could not be found");

            return await handler.HandleAsync((dynamic)command);
        }

        /// <summary>
        /// Returns the types of commands that can be processed.
        /// </summary>
        /// <returns>Supported commands</returns>
        public IEnumerable<Type> GetCommandTypes()
        {
            return _typeCollection.GetTypes();
        }

        public Type GetCommandType(string commandName)
        {
            return _typeCollection.GetType(commandName);
        }
    }
}