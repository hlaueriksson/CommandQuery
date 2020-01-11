using System;

namespace CommandQuery.Exceptions
{
    /// <summary>
    /// Represents errors that occur during query processing in a query handler.
    /// Derive custom query exceptions from this class and add public properties to expose more error details.
    /// </summary>
    public class QueryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public QueryException(string message) : base(message)
        {
        }
    }
}