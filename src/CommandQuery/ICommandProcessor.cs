using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task ProcessAsync(ICommand command);

        /// <summary>
        /// Process a command with result.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>The result of the command.</returns>
        Task<TResult> ProcessWithResultAsync<TResult>(ICommand<TResult> command);

        /// <summary>
        /// Returns the types of commands that can be processed.
        /// </summary>
        /// <returns>Supported command types.</returns>
        IEnumerable<Type> GetCommandTypes();

        /// <summary>
        /// Returns the type of command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <returns>The command type.</returns>
        Type GetCommandType(string commandName);
    }
}
