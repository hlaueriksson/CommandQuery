using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommandQuery.Client
{
    /// <summary>
    /// Client for sending commands to CommandQuery APIs over <c>HTTP</c>.
    /// </summary>
    public class CommandClient : BaseClient, ICommandClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="timeoutInSeconds">The timeout for requests.</param>
        public CommandClient(string baseUrl, int timeoutInSeconds = 10) : base(baseUrl, timeoutInSeconds)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="configAction">Configuration for the <see cref="HttpClient"/>.</param>
        public CommandClient(string baseUrl, Action<HttpClient> configAction) : base(baseUrl, configAction)
        {
        }

        /// <summary>
        /// Sends an <see cref="ICommand"/> to the API with <c>POST</c>.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task PostAsync(ICommand command) => await BasePostAsync(command).ConfigureAwait(false);

        /// <summary>
        /// Sends an <see cref="ICommand{TResult}"/> to the API with <c>POST</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>A result.</returns>
        public async Task<TResult> PostAsync<TResult>(ICommand<TResult> command) => await BasePostAsync<TResult>(command).ConfigureAwait(false);
    }
}
