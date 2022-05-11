#if NET6_0
using System;
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
        /// <returns>The result for status code <c>200</c>, or an error for status code <c>400</c> and <c>500</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="req"/> is <see langword="null"/>.</exception>
        Task<HttpResponseData> HandleAsync(string commandName, HttpRequestData req, ILogger? logger);
    }
}
#endif
