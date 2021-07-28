using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandClient"/> class.
        /// </summary>
        /// <param name="client">A <see cref="HttpClient"/>.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during serialization and deserialization of JSON.</param>
        public CommandClient(HttpClient client, JsonSerializerOptions? options = null) : base(client, options)
        {
        }

        /// <inheritdoc />
        public async Task PostAsync(ICommand command, CancellationToken cancellationToken = default) => await BasePostAsync(command, cancellationToken).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<TResult?> PostAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default) => await BasePostAsync<TResult>(command, cancellationToken).ConfigureAwait(false);
    }
}
