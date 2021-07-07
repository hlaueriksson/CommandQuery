using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;

namespace CommandQuery
{
    /// <inheritdoc />
    public class CommandProcessor : ICommandProcessor
    {
        private readonly ICommandTypeProvider _commandTypeProvider;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProcessor"/> class.
        /// </summary>
        /// <param name="commandTypeProvider">A provider of supported commands.</param>
        /// <param name="serviceProvider">A service provider with supported command handlers.</param>
        public CommandProcessor(ICommandTypeProvider commandTypeProvider, IServiceProvider serviceProvider)
        {
            _commandTypeProvider = commandTypeProvider;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public Type? GetCommandType(string commandName)
        {
            return _commandTypeProvider.GetCommandType(commandName);
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetCommandTypes()
        {
            return _commandTypeProvider.GetCommandTypes();
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
