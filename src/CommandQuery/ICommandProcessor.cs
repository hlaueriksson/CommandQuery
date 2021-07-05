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
        /// <param name="command">The command.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandProcessorException">The command handler for <paramref name="command"/> could not be found.</exception>
        Task ProcessAsync(ICommand command);

        /// <summary>
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>The result of the command.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandProcessorException">The command handler for <paramref name="command"/> could not be found.</exception>
        Task<TResult> ProcessWithResultAsync<TResult>(ICommand<TResult> command);

        /// <summary>
        /// Returns the type of command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>The command type.</returns>
        Type? GetCommandType(string commandName);

        /// <summary>
        /// Returns the types of commands that can be processed.
        /// </summary>
        /// <returns>Supported command types.</returns>
        IReadOnlyList<Type> GetCommandTypes();
    }
}
