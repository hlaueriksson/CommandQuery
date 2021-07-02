using System;
using System.Runtime.Serialization;

namespace CommandQuery.Exceptions
{
    /// <summary>
    /// Represents errors that occur during query processing in the <see cref="QueryProcessor" />.
    /// </summary>
    [Serializable]
    public sealed class QueryProcessorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryProcessorException"/> class.
        /// </summary>
        public QueryProcessorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryProcessorException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public QueryProcessorException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="QueryProcessorException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public QueryProcessorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        private QueryProcessorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
