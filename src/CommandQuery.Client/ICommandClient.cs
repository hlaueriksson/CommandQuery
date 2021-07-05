using System;
using System.Threading.Tasks;

namespace CommandQuery.Client
{
    /// <summary>
    /// Client for sending commands to CommandQuery APIs over <c>HTTP</c>.
    /// </summary>
    public interface ICommandClient
    {
        /// <summary>
        /// Sends an <see cref="ICommand"/> to the API with <c>POST</c>.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandQueryException">The <see cref="ICommand"/> failed.</exception>
        Task PostAsync(ICommand command);

        /// <summary>
        /// Sends an <see cref="ICommand{TResult}"/> to the API with <c>POST</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>A result.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandQueryException">The <see cref="ICommand{TResult}"/> failed.</exception>
        Task<TResult> PostAsync<TResult>(ICommand<TResult> command);
    }
}
