using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.NewtonsoftJson.Internal;
using Newtonsoft.Json.Linq;

namespace CommandQuery.NewtonsoftJson
{
    /// <summary>
    /// Extensions methods for <see cref="IQueryProcessor"/>.
    /// </summary>
    public static class QueryProcessorExtensions
    {
        /// <summary>
        /// Process a query.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="queryProcessor">The query processor.</param>
        /// <param name="queryName">The name of the query.</param>
        /// <param name="json">The JSON representation of the query.</param>
        /// <returns>The result of the query.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="queryProcessor"/> is <see langword="null"/>.</exception>
        /// <exception cref="QueryProcessorException">The process of the query failed.</exception>
        public static async Task<TResult> ProcessAsync<TResult>(this IQueryProcessor queryProcessor, string queryName, string json)
        {
            return await queryProcessor.ProcessAsync<TResult>(queryName, JObject.Parse(json)).ConfigureAwait(false);
        }

        /// <summary>
        /// Process a query.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="queryProcessor">The query processor.</param>
        /// <param name="queryName">The name of the query.</param>
        /// <param name="json">The JSON representation of the query.</param>
        /// <returns>The result of the query.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="queryProcessor"/> is <see langword="null"/>.</exception>
        /// <exception cref="QueryProcessorException">The process of the query failed.</exception>
        public static async Task<TResult> ProcessAsync<TResult>(this IQueryProcessor queryProcessor, string queryName, JObject json)
        {
            if (queryProcessor is null)
            {
                throw new ArgumentNullException(nameof(queryProcessor));
            }

            if (json is null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var queryType = queryProcessor.GetQueryType(queryName);

            if (queryType is null)
            {
                throw new QueryProcessorException($"The query type '{queryName}' could not be found");
            }

            var query = json.SafeToObject(queryType);

            if (query is null)
            {
                throw new QueryProcessorException("The json could not be converted to an object");
            }

            return await queryProcessor.ProcessAsync((dynamic)query);
        }

        /// <summary>
        /// Process a query.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="queryProcessor">The query processor.</param>
        /// <param name="queryName">The name of the query.</param>
        /// <param name="dictionary">The key/value representation of the query.</param>
        /// <returns>The result of the query.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="queryProcessor"/> is <see langword="null"/>.</exception>
        /// <exception cref="QueryProcessorException">The process of the query failed.</exception>
        public static async Task<TResult> ProcessAsync<TResult>(this IQueryProcessor queryProcessor, string queryName, IDictionary<string, IEnumerable<string>> dictionary)
        {
            if (queryProcessor is null)
            {
                throw new ArgumentNullException(nameof(queryProcessor));
            }

            var queryType = queryProcessor.GetQueryType(queryName);

            if (queryType is null)
            {
                throw new QueryProcessorException($"The query type '{queryName}' could not be found");
            }

            var query = GetQueryDictionary(dictionary, queryType).SafeToObject(queryType);

            if (query is null)
            {
                throw new QueryProcessorException("The dictionary could not be converted to an object");
            }

            return await queryProcessor.ProcessAsync((dynamic)query);
        }

        private static Dictionary<string, JToken>? GetQueryDictionary(IDictionary<string, IEnumerable<string>>? query, Type type)
        {
            if (query is null)
            {
                return null;
            }

            var properties = type.GetProperties();

            return query.ToDictionary(g => g.Key, Token, StringComparer.OrdinalIgnoreCase);

            JToken Token(KeyValuePair<string, IEnumerable<string>> kv)
            {
                var property = properties.FirstOrDefault(x => string.Equals(x.Name, kv.Key, StringComparison.OrdinalIgnoreCase));
                var isEnumerable = property?.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property?.PropertyType);

                return isEnumerable ? (JToken)new JArray(kv.Value) : kv.Value.FirstOrDefault();
            }
        }
    }
}
