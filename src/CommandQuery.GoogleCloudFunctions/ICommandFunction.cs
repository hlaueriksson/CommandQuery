using Microsoft.AspNetCore.Http;

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
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        Task HandleAsync(string commandName, HttpContext context, CancellationToken cancellationToken = default);
    }
}
