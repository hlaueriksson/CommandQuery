using System;

namespace CommandQuery.Exceptions
{
    /// <summary>
    /// Represents errors that occur during query processing.
    /// </summary>
    public sealed class QueryProcessorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryProcessorException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public QueryProcessorException(string message) : base(message)
        {
        }
    }
}