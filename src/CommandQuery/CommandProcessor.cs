using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public async Task ProcessAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic? handler = GetService(handlerType);

            if (handler is null)
            {
                throw new CommandProcessorException($"The command handler for '{command}' could not be found.");
            }

            await handler.HandleAsync((dynamic)command, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TResult> ProcessAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));

            dynamic? handler = GetService(handlerType);

            if (handler is null)
            {
                throw new CommandProcessorException($"The command handler for '{command}' could not be found.");
            }

            return await handler.HandleAsync((dynamic)command, cancellationToken);
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

        /// <inheritdoc />
        public ICommandProcessor AssertConfigurationIsValid()
        {
            var errors = new List<string>();

            foreach (var commandType in GetCommandTypes())
            {
                var handlerType = commandType.IsAssignableTo(typeof(ICommand))
                    ? typeof(ICommandHandler<>).MakeGenericType(commandType)
                    : typeof(ICommandHandler<,>).MakeGenericType(commandType, commandType.GetResultType(typeof(ICommand<>)));

                try
                {
                    if (GetService(handlerType) is null)
                    {
                        errors.Add($"The command handler for '{commandType.AssemblyQualifiedName}' is not registered.");
                    }
                }
                catch (CommandProcessorException)
                {
                    errors.Add($"A single command handler for '{commandType.AssemblyQualifiedName}' could not be retrieved.");
                }
            }

            foreach (var handlerType in _serviceProvider
                .GetAllServiceTypes()
                .Where(x => x.IsGenericType && (x.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || x.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .ToList())
            {
                var commandType = handlerType.GetGenericArguments()[0];
                var supportedCommandType = _commandTypeProvider.GetCommandType(commandType.Name);

                if (supportedCommandType is null || supportedCommandType != commandType)
                {
                    errors.Add($"The command '{commandType.AssemblyQualifiedName}' is not registered.");
                }
            }

            if (errors.Any())
            {
                throw new CommandTypeException("The CommandProcessor configuration is not valid:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
            }

            return this;
        }

        private object? GetService(Type handlerType)
        {
            try
            {
                return _serviceProvider.GetSingleService(handlerType);
            }
            catch (InvalidOperationException exception)
            {
                throw new CommandProcessorException($"A single command handler for '{handlerType}' could not be retrieved.", exception);
            }
        }
    }
}
