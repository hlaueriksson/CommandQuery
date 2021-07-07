using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <returns>The result + 200, 400 or 500.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        Task HandleAsync(string queryName, HttpContext context, ILogger logger);
    }
}
