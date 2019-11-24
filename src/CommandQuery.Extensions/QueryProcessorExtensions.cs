using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using Newtonsoft.Json.Linq;

namespace CommandQuery
{
    public static class QueryProcessorExtensions
    {
        /// <summary>
        /// Process a query.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="queryProcessor">The query processor</param>
        /// <param name="queryName">The name of the query</param>
        /// <param name="json">The JSON representation of the query</param>
        /// <returns>The result of the query</returns>
        public static async Task<TResult> ProcessAsync<TResult>(this IQueryProcessor queryProcessor, string queryName, string json)
        {
            return await queryProcessor.ProcessAsync<TResult>(queryName, JObject.Parse(json));
        }

        /// <summary>
        /// Process a query.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="queryProcessor">The query processor</param>
        /// <param name="queryName">The name of the query</param>
        /// <param name="json">The JSON representation of the query</param>
        /// <returns>The result of the query</returns>
        public static async Task<TResult> ProcessAsync<TResult>(this IQueryProcessor queryProcessor, string queryName, JObject json)
        {
            var queryType = queryProcessor.GetQueryType(queryName);

            if (queryType == null) throw new QueryProcessorException($"The query type '{queryName}' could not be found");

            var query = json.SafeToObject(queryType);

            if (query == null) throw new QueryProcessorException("The json could not be converted to an object");

            return await queryProcessor.ProcessAsync((dynamic)query);
        }

        /// <summary>
        /// Process a query.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="queryProcessor">The query processor</param>
        /// <param name="queryName">The name of the query</param>
        /// <param name="dictionary">The key/value representation of the query</param>
        /// <returns>The result of the query</returns>
        public static async Task<TResult> ProcessAsync<TResult>(this IQueryProcessor queryProcessor, string queryName, IDictionary<string, IEnumerable<string>> dictionary)
        {
            var queryType = queryProcessor.GetQueryType(queryName);

            if (queryType == null) throw new QueryProcessorException($"The query type '{queryName}' could not be found");

            var query = GetQueryDictionary(dictionary, queryType).SafeToObject(queryType);

            if (query == null) throw new QueryProcessorException("The dictionary could not be converted to an object");

            return await queryProcessor.ProcessAsync((dynamic)query);
        }

        private static Dictionary<string, JToken> GetQueryDictionary(IDictionary<string, IEnumerable<string>> query, Type type)
        {
            if (query == null) return null;

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