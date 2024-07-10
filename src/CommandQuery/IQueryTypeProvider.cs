namespace CommandQuery
{
    /// <summary>
    /// A provider of supported query types.
    /// </summary>
    public interface IQueryTypeProvider
    {
        /// <summary>
        /// Returns the type of query.
        /// </summary>
        /// <param name="key">The type key.</param>
        /// <returns>The type of query.</returns>
        Type? GetQueryType(string key);

        /// <summary>
        /// Returns the types of supported queries.
        /// </summary>
        /// <returns>Supported queries.</returns>
        IReadOnlyList<Type> GetQueryTypes();
    }
}
