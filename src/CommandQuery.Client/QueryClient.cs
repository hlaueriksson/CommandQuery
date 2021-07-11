using System;
using System.Net.Http;
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
        public QueryClient(string baseUrl, int timeoutInSeconds = 10) : base(baseUrl, timeoutInSeconds)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="configAction">Configuration for the <see cref="HttpClient"/>.</param>
        public QueryClient(string baseUrl, Action<HttpClient> configAction) : base(baseUrl, configAction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryClient"/> class.
        /// </summary>
        /// <param name="client">A <see cref="HttpClient"/>.</param>
        public QueryClient(HttpClient client) : base(client)
        {
        }

        /// <inheritdoc />
        public async Task<TResult?> GetAsync<TResult>(IQuery<TResult> query) => await BaseGetAsync<TResult>(query).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<TResult?> PostAsync<TResult>(IQuery<TResult> query) => await BasePostAsync<TResult>(query).ConfigureAwait(false);
    }
}
