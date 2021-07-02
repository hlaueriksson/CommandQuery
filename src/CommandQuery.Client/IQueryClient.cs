using System.Threading.Tasks;

namespace CommandQuery.Client
{
    /// <summary>
    /// Client for sending queries to CommandQuery APIs over <c>HTTP</c>.
    /// </summary>
    public interface IQueryClient
    {
        /// <summary>
        /// Sends an <see cref="IQuery{TResult}" /> to the API with <c>GET</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        Task<TResult> GetAsync<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Sends an <see cref="IQuery{TResult}" /> to the API with <c>POST</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        Task<TResult> PostAsync<TResult>(IQuery<TResult> query);
    }
}
