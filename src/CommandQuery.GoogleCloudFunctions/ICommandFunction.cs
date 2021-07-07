using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CommandQuery.GoogleCloudFunctions
{
    /// <summary>
    /// Handles commands for the Google Cloud function.
    /// </summary>
    public interface ICommandFunction
    {
        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="context">A <see cref="HttpContext"/>.</param>
        /// <param name="log">An <see cref="ILogger"/>.</param>
        /// <returns>200, 400 or 500.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        Task HandleAsync(string commandName, HttpContext context, ILogger log);
    }
}
