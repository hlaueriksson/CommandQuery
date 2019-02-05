using System.Threading.Tasks;

namespace CommandQuery
{
    /// <summary>
    /// Defines a handler for a query.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being handled</typeparam>
    /// <typeparam name="TResult">The type of result from the handler</typeparam>
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Handles a query.
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>Result from the query</returns>
        Task<TResult> HandleAsync(TQuery query);
    }
}