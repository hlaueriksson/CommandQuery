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
        /// Initializes a new instance of the <see cref="QueryException"/> class.
        /// </summary>
        public QueryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public QueryException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="QueryException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public QueryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
