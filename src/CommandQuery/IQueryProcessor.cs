using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;

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
        /// <exception cref="ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
        /// <exception cref="QueryProcessorException">The query handler for <paramref name="query"/> could not be found.</exception>
        Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Returns the type of query.
        /// </summary>
        /// <param name="queryName">The name of the query.</param>
        /// <returns>The query type.</returns>
        Type? GetQueryType(string queryName);

        /// <summary>
        /// Returns the types of queries that can be processed.
        /// </summary>
        /// <returns>Supported query types.</returns>
        IReadOnlyList<Type> GetQueryTypes();
    }
}
