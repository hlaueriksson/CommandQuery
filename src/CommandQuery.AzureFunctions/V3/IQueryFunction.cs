#if NETCOREAPP3_1
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AzureFunctions
{
    /// <summary>
    /// Handles queries for the Azure function.
    /// </summary>
    public interface IQueryFunction
    {
        /// <summary>
        /// Handle a query.
        /// </summary>
        /// <param name="queryName">The name of the query.</param>
        /// <param name="req">A <see cref="HttpRequest"/>.</param>
        /// <param name="log">An <see cref="ILogger"/>.</param>
        /// <returns>The result + 200, 400 or 500.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="req"/> is <see langword="null"/>.</exception>
        Task<IActionResult> HandleAsync(string queryName, HttpRequest req, ILogger log);
    }
}
#endif
