using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CommandQuery.Client
{
    /// <inheritdoc cref="IQueryClient" />
    public class QueryClient : BaseClient, IQueryClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="timeoutInSeconds">The timeout for requests.</param>
        public QueryClient(string baseUrl, int timeoutInSeconds = 10)
            : base(baseUrl, timeoutInSeconds)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="configAction">Configuration for the <see cref="HttpClient"/>.</param>
        public QueryClient(string baseUrl, Action<HttpClient> configAction)
            : base(baseUrl, configAction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryClient"/> class.
        /// </summary>
        /// <param name="client">A <see cref="HttpClient"/>.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during serialization and deserialization of JSON.</param>
        public QueryClient(HttpClient client, JsonSerializerOptions? options = null)
            : base(client, options)
        {
        }

        /// <inheritdoc />
        public async Task<TResult?> GetAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) => await BaseGetAsync<TResult>(query, cancellationToken).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<TResult?> PostAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) => await BasePostAsync<TResult>(query, cancellationToken).ConfigureAwait(false);
    }
}
