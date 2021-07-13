using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CommandQuery.Client
{
    /// <summary>
    /// Base class for clients to CommandQuery APIs.
    /// </summary>
    public abstract class BaseClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="timeoutInSeconds">The timeout for requests.</param>
        protected BaseClient(string baseUrl, int timeoutInSeconds = 10)
        {
            Client.BaseAddress = new Uri(baseUrl);
            Client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="configAction">Configuration for the <see cref="HttpClient"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="configAction"/> is <see langword="null"/>.</exception>
        protected BaseClient(string baseUrl, Action<HttpClient> configAction) : this(baseUrl)
        {
            if (configAction is null)
            {
                throw new ArgumentNullException(nameof(configAction));
            }

            configAction(Client);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient"/> class.
        /// </summary>
        /// <param name="client">A <see cref="HttpClient"/>.</param>
        /// <param name="options"><see cref="JsonSerializerOptions"/> to control the behavior during serialization and deserialization of JSON.</param>
        protected BaseClient(HttpClient client, JsonSerializerOptions? options)
        {
            Client = client;
            Options = options;
        }

        /// <summary>
        /// Sends HTTP requests and receives HTTP responses.
        /// </summary>
        protected HttpClient Client { get; } = new();

        /// <summary>
        /// Options to control the behavior during serialization and deserialization of JSON.
        /// </summary>
        protected JsonSerializerOptions? Options { get; }

        /// <summary>
        /// Gets a result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="value">A payload.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A result.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandQueryException">The <c>GET</c> request failed.</exception>
        protected async Task<T?> BaseGetAsync<T>(object value, CancellationToken cancellationToken)
        {
            var response = await Client.GetAsync(value.GetRequestUri(), cancellationToken).ConfigureAwait(false);
            await response.EnsureSuccessAsync(cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadFromJsonAsync<T>(Options, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Post a payload.
        /// </summary>
        /// <param name="value">A payload.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandQueryException">The <c>POST</c> request failed.</exception>
        protected async Task BasePostAsync(object value, CancellationToken cancellationToken)
        {
            var response = await Client.PostAsJsonAsync(value.GetRequestSlug(), value, Options, cancellationToken).ConfigureAwait(false);
            await response.EnsureSuccessAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Post a payload and returns a result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="value">A payload.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A result.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandQueryException">The <c>POST</c> request failed.</exception>
        protected async Task<T?> BasePostAsync<T>(object value, CancellationToken cancellationToken)
        {
            var response = await Client.PostAsJsonAsync(value.GetRequestSlug(), value, Options, cancellationToken).ConfigureAwait(false);
            await response.EnsureSuccessAsync(cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadFromJsonAsync<T>(Options, cancellationToken).ConfigureAwait(false);
        }
    }
}
