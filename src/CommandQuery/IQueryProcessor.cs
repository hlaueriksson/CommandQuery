using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandQuery
{
    /// <summary>
    /// Process queries by invoking the corresponding handler.
    /// </summary>
    public interface IQueryProcessor
    {
        /// <summary>
        /// Process a query.
        /// </summary>
        /// <typeparam name="TResult">The type of result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>The result of the query.</returns>
        Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Returns the types of queries that can be processed.
        /// </summary>
        /// <returns>Supported query types.</returns>
        IEnumerable<Type> GetQueryTypes();

        /// <summary>
        /// Returns the type of query.
        /// </summary>
        /// <param name="queryName">The name of the query.</param>
        /// <returns>The query type.</returns>
        Type? GetQueryType(string queryName);
    }
}
