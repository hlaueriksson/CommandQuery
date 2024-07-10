namespace CommandQuery.Client
{
    /// <summary>
    /// Client for sending queries to CommandQuery APIs over <c>HTTP</c>.
    /// </summary>
    public interface IQueryClient
    {
        /// <summary>
        /// Sends an <see cref="IQuery{TResult}"/> to the API with <c>GET</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A result.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandQueryException">The <see cref="IQuery{TResult}"/> failed.</exception>
        Task<TResult?> GetAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an <see cref="IQuery{TResult}"/> to the API with <c>POST</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A result.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandQueryException">The <see cref="IQuery{TResult}"/> failed.</exception>
        Task<TResult?> PostAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    }
}
