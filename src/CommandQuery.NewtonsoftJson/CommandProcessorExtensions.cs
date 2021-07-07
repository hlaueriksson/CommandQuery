using System;
using System.Threading.Tasks;
using CommandQuery.Exceptions;

namespace CommandQuery.NewtonsoftJson
{
    /// <summary>
    /// Extensions methods for <see cref="ICommandProcessor"/>.
    /// </summary>
    public static class CommandProcessorExtensions
    {
        /// <summary>
        /// Process a command with or without result.
        /// </summary>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="json">The JSON representation of the command.</param>
        /// <returns>The result of the command wrapped in a <see cref="CommandResult"/>, or <see cref="CommandResult.None"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commandProcessor"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandProcessorException">The process of the command failed.</exception>
        public static async Task<CommandResult> ProcessWithOrWithoutResultAsync(this ICommandProcessor commandProcessor, string commandName, string? json)
        {
            if (commandProcessor is null)
            {
                throw new ArgumentNullException(nameof(commandProcessor));
            }

            if (json is null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var command = commandProcessor.GetCommand(commandName, json);

            if (command is ICommand commandWithoutResult)
            {
                await commandProcessor.ProcessAsync(commandWithoutResult).ConfigureAwait(false);

                return CommandResult.None;
            }

            var result = await commandProcessor.ProcessWithResultAsync((dynamic)command);

            return new CommandResult(result);
        }

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="json">The JSON representation of the command.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commandProcessor"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandProcessorException">The process of the command failed.</exception>
        public static async Task ProcessAsync(this ICommandProcessor commandProcessor, string commandName, string json)
        {
            if (commandProcessor is null)
            {
                throw new ArgumentNullException(nameof(commandProcessor));
            }

            var command = commandProcessor.GetCommand(commandName, json);

            await commandProcessor.ProcessAsync((dynamic)command);
        }

        /// <summary>
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="json">The JSON representation of the command.</param>
        /// <returns>The result of the command.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commandProcessor"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandProcessorException">The process of the command failed.</exception>
        public static async Task<TResult> ProcessWithResultAsync<TResult>(this ICommandProcessor commandProcessor, string commandName, string json)
        {
            if (commandProcessor is null)
            {
                throw new ArgumentNullException(nameof(commandProcessor));
            }

            var command = commandProcessor.GetCommand(commandName, json);

            return await commandProcessor.ProcessWithResultAsync<TResult>((dynamic)command);
        }

        private static object GetCommand(this ICommandProcessor commandProcessor, string commandName, string json)
        {
            var commandType = commandProcessor.GetCommandType(commandName);

            if (commandType is null)
            {
                throw new CommandProcessorException($"The command type '{commandName}' could not be found");
            }

            var command = json.SafeToObject(commandType);

            if (command is null)
            {
                throw new CommandProcessorException("The json could not be converted to an object");
            }

            return command;
        }
    }
}
