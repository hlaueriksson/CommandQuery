using System.Threading.Tasks;

namespace CommandQuery
{
    /// <summary>
    /// Defines a handler for a command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Handles a command.
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task HandleAsync(TCommand command);
    }
}