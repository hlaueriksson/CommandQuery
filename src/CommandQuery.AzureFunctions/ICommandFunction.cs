using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommandQuery.AzureFunctions
{
    /// <summary>
    /// Handles commands for the Azure function.
    /// </summary>
    public interface ICommandFunction
    {
        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="req">A <see cref="HttpRequest"/>.</param>
        /// <param name="log">An <see cref="ILogger"/>.</param>
        /// <returns>200, 400 or 500.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="req"/> is <see langword="null"/>.</exception>
        Task<IActionResult> HandleAsync(string commandName, HttpRequest req, ILogger log);
    }
}
