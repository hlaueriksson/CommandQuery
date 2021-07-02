using System.Threading.Tasks;

namespace CommandQuery
{
    /// <summary>
    /// Defines a handler for a command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Handles a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task HandleAsync(TCommand command);
    }

    /// <summary>
    /// Defines a handler for a command with result.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    /// <typeparam name="TResult">The type of result from the handler.</typeparam>
    public interface ICommandHandler<in TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Handles a command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>Result from the command.</returns>
        Task<TResult> HandleAsync(TCommand command);
    }
}
