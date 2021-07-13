#if NET5_0
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
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
        /// <param name="req">A <see cref="HttpRequestData"/>.</param>
        /// <param name="logger">An <see cref="ILogger"/>.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>200, 400 or 500.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="req"/> is <see langword="null"/>.</exception>
        Task<HttpResponseData> HandleAsync(string commandName, HttpRequestData req, ILogger? logger, CancellationToken cancellationToken = default);
    }
}
#endif
