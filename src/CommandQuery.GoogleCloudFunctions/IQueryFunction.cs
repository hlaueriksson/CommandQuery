using Microsoft.AspNetCore.Http;

namespace CommandQuery.GoogleCloudFunctions
{
    /// <summary>
    /// Handles queries for the Google Cloud function.
    /// </summary>
    public interface IQueryFunction
    {
        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query.</param>
        /// <param name="context">A <see cref="HttpContext"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        Task HandleAsync(string queryName, HttpContext context, CancellationToken cancellationToken = default);
    }
}
