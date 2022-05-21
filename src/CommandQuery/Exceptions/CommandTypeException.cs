using System;
using System.Runtime.Serialization;

namespace CommandQuery.Exceptions
{
    /// <summary>
    /// Represents errors that occur during initialization of a <see cref="CommandTypeProvider"/>.
    /// </summary>
    [Serializable]
    public sealed class CommandTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTypeException"/> class.
        /// </summary>
        public CommandTypeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTypeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommandTypeException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="CommandTypeException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public CommandTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private CommandTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
