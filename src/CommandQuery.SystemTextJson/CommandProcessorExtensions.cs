using System.Text.Json;
using CommandQuery.Exceptions;

namespace CommandQuery.SystemTextJson
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
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during deserialization of <paramref name="json"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The result of the command wrapped in a <see cref="CommandResult"/>, or <see cref="CommandResult.None"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commandProcessor"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandProcessorException">The process of the command failed.</exception>
        public static async Task<CommandResult> ProcessAsync(this ICommandProcessor commandProcessor, string commandName, string? json, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (commandProcessor is null)
            {
                throw new ArgumentNullException(nameof(commandProcessor));
            }

            if (json is null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var commandType = commandProcessor.GetCommandType(commandName);

            if (commandType is null)
            {
                throw new CommandProcessorException($"The command type '{commandName}' could not be found");
            }

            var command = json.SafeDeserialize(commandType, options);

            switch (command)
            {
                case null:
                    throw new CommandProcessorException("The json string could not be deserialized to an object");
                case ICommand commandWithoutResult:
                    await commandProcessor.ProcessAsync(commandWithoutResult, cancellationToken).ConfigureAwait(false);
                    return CommandResult.None;
                default:
                    var commandWithResult = (dynamic)command;
                    var result = await commandProcessor.ProcessAsync(commandWithResult, cancellationToken).ConfigureAwait(false);
                    return new CommandResult(result);
            }
        }
    }
}
