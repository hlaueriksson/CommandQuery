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
        /// Sends HTTP requests and receives HTTP responses.
        /// </summary>
        protected readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient" /> class.
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
        /// Initializes a new instance of the <see cref="BaseClient" /> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="configAction">Configuration for the <see cref="HttpClient" />.</param>
        protected BaseClient(string baseUrl, Action<HttpClient> configAction) : this(baseUrl)
        {
            configAction(Client);
        }

        /// <summary>
        /// Gets a result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="value">A payload.</param>
        /// <returns>A result.</returns>
        protected T BaseGet<T>(object value)
            => Client.GetAsync(value.GetRequestUri())
                .ConfigureAwait(false).GetAwaiter().GetResult()
                .EnsureSuccessStatusCode()
                .Content.ReadAsAsync<T>()
                .ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>
        /// Gets a result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="value">A payload.</param>
        /// <returns>A result.</returns>
        protected async Task<T> BaseGetAsync<T>(object value)
        {
            var response = await Client.GetAsync(value.GetRequestUri());
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        /// <summary>
        /// Post a payload.
        /// </summary>
        /// <param name="value">A payload.</param>
        protected void BasePost(object value)
            => Client.PostAsJsonAsync(value.GetType().Name, value)
                .ConfigureAwait(false).GetAwaiter().GetResult()
                .EnsureSuccessStatusCode();

        /// <summary>
        /// Post a payload.
        /// </summary>
        /// <param name="value">A payload.</param>
        protected async Task BasePostAsync(object value)
        {
            var response = await Client.PostAsJsonAsync(value.GetType().Name, value);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Post a payload and returns a result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="value">A payload.</param>
        /// <returns>A result.</returns>
        protected T BasePost<T>(object value)
            => Client.PostAsJsonAsync(value.GetType().Name, value)
                .ConfigureAwait(false).GetAwaiter().GetResult()
                .EnsureSuccessStatusCode()
                .Content.ReadAsAsync<T>()
                .ConfigureAwait(false).GetAwaiter().GetResult();

        /// <summary>
        /// Post a payload and returns a result.
        /// </summary>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <param name="value">A payload.</param>
        /// <returns>A result.</returns>
        protected async Task<T> BasePostAsync<T>(object value)
        {
            var response = await Client.PostAsJsonAsync(value.GetType().Name, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }
    }
}