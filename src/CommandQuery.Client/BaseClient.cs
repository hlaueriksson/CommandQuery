using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CommandQuery.Client.Internal;

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
        /// Sends HTTP requests and receives HTTP responses.
        /// </summary>
        protected HttpClient Client { get; } = new();

        /// <summary>
        /// Gets a result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="value">A payload.</param>
        /// <returns>A result.</returns>
        protected async Task<T> BaseGetAsync<T>(object value)
        {
#pragma warning disable CA2234 // Pass system uri objects instead of strings
            var response = await Client.GetAsync(value.GetRequestUri()).ConfigureAwait(false);
#pragma warning restore CA2234 // Pass system uri objects instead of strings
            response.EnsureSuccess();
            return await response.Content.ReadAsAsync<T>().ConfigureAwait(false);
        }

        /// <summary>
        /// Post a payload.
        /// </summary>
        /// <param name="value">A payload.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task BasePostAsync(object value)
        {
            var response = await Client.PostAsJsonAsync(value.GetRequestSlug(), value).ConfigureAwait(false);
            response.EnsureSuccess();
        }

        /// <summary>
        /// Post a payload and returns a result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="value">A payload.</param>
        /// <returns>A result.</returns>
        protected async Task<T> BasePostAsync<T>(object value)
        {
            var response = await Client.PostAsJsonAsync(value.GetRequestSlug(), value).ConfigureAwait(false);
            response.EnsureSuccess();
            return await response.Content.ReadAsAsync<T>().ConfigureAwait(false);
        }
    }
}
