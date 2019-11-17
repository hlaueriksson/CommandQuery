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
        /// Process a command with or without result.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command wrapped in a <see cref="CommandResult"/>, or <see cref="CommandResult.None"/></returns>
        Task<CommandResult> ProcessWithOrWithoutResultAsync(string commandName, string json);

        /// <summary>
        /// Process a command with or without result.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command wrapped in a <see cref="CommandResult"/>, or <see cref="CommandResult.None"/></returns>
        Task<CommandResult> ProcessWithOrWithoutResultAsync(string commandName, JObject json);

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
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command</returns>
        Task<TResult> ProcessWithResultAsync<TResult>(string commandName, string json);

        /// <summary>
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command</returns>
        Task<TResult> ProcessWithResultAsync<TResult>(string commandName, JObject json);

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
        /// Process a command with or without result.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command wrapped in a <see cref="CommandResult"/>, or <see cref="CommandResult.None"/></returns>
        public async Task<CommandResult> ProcessWithOrWithoutResultAsync(string commandName, string json)
        {
            return await ProcessWithOrWithoutResultAsync(commandName, JObject.Parse(json));
        }

        /// <summary>
        /// Process a command with or without result.
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command wrapped in a <see cref="CommandResult"/>, or <see cref="CommandResult.None"/></returns>
        public async Task<CommandResult> ProcessWithOrWithoutResultAsync(string commandName, JObject json)
        {
            var command = GetCommand(commandName, json);

            if (command is ICommand commandWithoutResult)
            {
                await ProcessAsync(commandWithoutResult);

                return CommandResult.None;
            }

            var result = await ProcessWithResultAsync((dynamic)command);

            return new CommandResult(result);
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
            var command = GetCommand(commandName, json);

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
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command</returns>
        public async Task<TResult> ProcessWithResultAsync<TResult>(string commandName, string json)
        {
            return await ProcessWithResultAsync<TResult>(commandName, JObject.Parse(json));
        }

        /// <summary>
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="commandName">The name of the command</param>
        /// <param name="json">The JSON representation of the command</param>
        /// <returns>The result of the command</returns>
        public async Task<TResult> ProcessWithResultAsync<TResult>(string commandName, JObject json)
        {
            var command = GetCommand(commandName, json);

            return await ProcessWithResultAsync<TResult>((dynamic)command);
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
        public IEnumerable<Type> GetCommands()
        {
            return _typeCollection.GetTypes();
        }

        private object GetCommand(string commandName, JObject json)
        {
            var commandType = _typeCollection.GetType(commandName);

            if (commandType == null) throw new CommandProcessorException($"The command type '{commandName}' could not be found");

            var command = json.SafeToObject(commandType);

            if (command == null) throw new CommandProcessorException("The json could not be converted to an object");

            return command;
        }
    }
}