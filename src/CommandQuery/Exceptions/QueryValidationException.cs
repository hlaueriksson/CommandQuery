using System;

namespace CommandQuery.Exceptions
{
    /// <summary>
    /// Represents errors that occur during query validation.
    /// </summary>
    public class QueryValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryValidationException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public QueryValidationException(string message) : base(message)
        {
        }
    }
}