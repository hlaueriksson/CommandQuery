using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommandQuery.Client
{
    /// <inheritdoc cref="ICommandClient" />
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


        /// <inheritdoc />
        public async Task PostAsync(ICommand command) => await BasePostAsync(command).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<TResult> PostAsync<TResult>(ICommand<TResult> command) => await BasePostAsync<TResult>(command).ConfigureAwait(false);
    }
}
