using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using Newtonsoft.Json.Linq;

namespace CommandQuery
{
    public static class CommandProcessorExtensions
    {
        /// <summary>
        /// Process a command with or without result.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command wrapped in a <see cref="CommandResult"/>, or <see cref="CommandResult.None"/></returns>
        public static async Task<CommandResult> ProcessWithOrWithoutResultAsync(this ICommandProcessor commandProcessor, string commandName, string json)
        {
            return await commandProcessor.ProcessWithOrWithoutResultAsync(commandName, JObject.Parse(json));
        }

        /// <summary>
        /// Process a command with or without result.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command wrapped in a <see cref="CommandResult"/>, or <see cref="CommandResult.None"/></returns>
        public static async Task<CommandResult> ProcessWithOrWithoutResultAsync(this ICommandProcessor commandProcessor, string commandName, JObject json)
        {
            var command = commandProcessor.GetCommand(commandName, json);

            if (command is ICommand commandWithoutResult)
            {
                await commandProcessor.ProcessAsync(commandWithoutResult);

                return CommandResult.None;
            }

            var result = await commandProcessor.ProcessWithResultAsync((dynamic)command);

            return new CommandResult(result);
        }

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task ProcessAsync(this ICommandProcessor commandProcessor, string commandName, string json)
        {
            await commandProcessor.ProcessAsync(commandName, JObject.Parse(json));
        }

        /// <summary>
        /// Process a command.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task ProcessAsync(this ICommandProcessor commandProcessor, string commandName, JObject json)
        {
            var command = commandProcessor.GetCommand(commandName, json);

            await commandProcessor.ProcessAsync((dynamic)command);
        }

        /// <summary>
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command</returns>
        public static async Task<TResult> ProcessWithResultAsync<TResult>(this ICommandProcessor commandProcessor, string commandName, string json)
        {
            return await commandProcessor.ProcessWithResultAsync<TResult>(commandName, JObject.Parse(json));
        }

        /// <summary>
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command</returns>
        public static async Task<TResult> ProcessWithResultAsync<TResult>(this ICommandProcessor commandProcessor, string commandName, JObject json)
        {
            var command = commandProcessor.GetCommand(commandName, json);

            return await commandProcessor.ProcessWithResultAsync<TResult>((dynamic)command);
        }

        private static object GetCommand(this ICommandProcessor commandProcessor, string commandName, JObject json)
        {
            var commandType = commandProcessor.GetCommandType(commandName);

            if (commandType == null) throw new CommandProcessorException($"The command type '{commandName}' could not be found");

            var command = json.SafeToObject(commandType);

            if (command == null) throw new CommandProcessorException("The json could not be converted to an object");

            return command;
        }
    }
}