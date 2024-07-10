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
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandQueryException">The <see cref="ICommand"/> failed.</exception>
        Task PostAsync(ICommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an <see cref="ICommand{TResult}"/> to the API with <c>POST</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A result.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandQueryException">The <see cref="ICommand{TResult}"/> failed.</exception>
        Task<TResult?> PostAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
    }
}
