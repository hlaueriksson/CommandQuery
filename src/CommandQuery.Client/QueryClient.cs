using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommandQuery.Client
{
    /// <summary>
    /// Client for sending queries to CommandQuery APIs over <c>HTTP</c>.
    /// </summary>
    public interface IQueryClient
    {
        /// <summary>
        /// Sends an <see cref="IQuery&lt;TResult&gt;" /> to the API with <c>GET</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        TResult Get<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Sends an <see cref="IQuery&lt;TResult&gt;" /> to the API with <c>GET</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        Task<TResult> GetAsync<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Sends an <see cref="IQuery&lt;TResult&gt;" /> to the API with <c>POST</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        TResult Post<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Sends an <see cref="IQuery&lt;TResult&gt;" /> to the API with <c>POST</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        Task<TResult> PostAsync<TResult>(IQuery<TResult> query);
    }

    /// <summary>
    /// Client for sending queries to CommandQuery APIs over <c>HTTP</c>.
    /// </summary>
    public class QueryClient : BaseClient, IQueryClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryClient" /> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="timeoutInSeconds">The timeout for requests.</param>
        public QueryClient(string baseUrl, int timeoutInSeconds = 10) : base(baseUrl, timeoutInSeconds) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryClient" /> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to the API.</param>
        /// <param name="configAction">Configuration for the <see cref="HttpClient" />.</param>
        public QueryClient(string baseUrl, Action<HttpClient> configAction) : base(baseUrl, configAction) { }

        /// <summary>
        /// Sends an <see cref="IQuery&lt;TResult&gt;" /> to the API with <c>GET</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        public TResult Get<TResult>(IQuery<TResult> query) => BaseGet<TResult>(query);

        /// <summary>
        /// Sends an <see cref="IQuery&lt;TResult&gt;" /> to the API with <c>GET</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        public async Task<TResult> GetAsync<TResult>(IQuery<TResult> query) => await BaseGetAsync<TResult>(query);

        /// <summary>
        /// Sends an <see cref="IQuery&lt;TResult&gt;" /> to the API with <c>POST</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        public TResult Post<TResult>(IQuery<TResult> query) => BasePost<TResult>(query);

        /// <summary>
        /// Sends an <see cref="IQuery&lt;TResult&gt;" /> to the API with <c>POST</c>.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>A result.</returns>
        public async Task<TResult> PostAsync<TResult>(IQuery<TResult> query) => await BasePostAsync<TResult>(query);
    }
}