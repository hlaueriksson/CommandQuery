using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Internal;

namespace CommandQuery
{
    /// <summary>
    /// Process commands by invoking the corresponding handler.
    /// </summary>
    public class CommandProcessor : ICommandProcessor
    {
        private readonly ITypeCollection _typeCollection;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProcessor"/> class.
        /// </summary>
        /// <param name="typeCollection">A collection of supported commands.</param>
        /// <param name="serviceProvider">A service provider with supported command handlers.</param>
        public CommandProcessor(ICommandTypeCollection typeCollection, IServiceProvider serviceProvider)
        {
            _typeCollection = typeCollection;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandProcessorException">The command handler for <paramref name="command"/> could not be found.</exception>
        public async Task ProcessAsync(ICommand command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic? handler = GetService(handlerType);

            if (handler is null)
            {
                throw new CommandProcessorException($"The command handler for '{command}' could not be found");
            }

            await handler.HandleAsync((dynamic)command);
        }

        /// <summary>
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>The result of the command.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandProcessorException">The command handler for <paramref name="command"/> could not be found.</exception>
        public async Task<TResult> ProcessWithResultAsync<TResult>(ICommand<TResult> command) // TODO: ProcessAsync?
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

            dynamic? handler = GetService(handlerType);

            if (handler is null)
            {
                throw new CommandProcessorException($"The command handler for '{command}' could not be found");
            }

            return await handler.HandleAsync((dynamic)command);
        }

        /// <summary>
        /// Returns the types of commands that can be processed.
        /// </summary>
        /// <returns>Supported command types.</returns>
        public IEnumerable<Type> GetCommandTypes()
        {
            return _typeCollection.GetTypes();
        }

        /// <summary>
        /// Returns the type of command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>The command type.</returns>
        public Type? GetCommandType(string commandName)
        {
            return _typeCollection.GetType(commandName);
        }

        private object? GetService(Type handlerType)
        {
            try
            {
                return _serviceProvider.GetSingleService(handlerType);
            }
            catch (InvalidOperationException)
            {
                throw new CommandProcessorException($"Multiple command handlers for '{handlerType}' was found");
            }
        }
    }
}
