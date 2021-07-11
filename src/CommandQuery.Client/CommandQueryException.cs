using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace CommandQuery.Client
{
    /// <summary>
    /// Represents errors that occur when receiving HTTP responses in the <see cref="CommandClient"/> and <see cref="QueryClient"/>.
    /// </summary>
    [Serializable]
    public class CommandQueryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandQueryException"/> class.
        /// </summary>
        public CommandQueryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandQueryException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommandQueryException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandQueryException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public CommandQueryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandQueryException"/> class with a specified error message and <see cref="IError"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="error">The error that occurred during the processing of a command or query.</param>
        public CommandQueryException(string message, IError? error) : base(message)
        {
            Error = error;
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("Error", Error, typeof(Error));

            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandQueryException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected CommandQueryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Error = info.GetValue("Error", typeof(Error)) as Error;
        }

        /// <summary>
        /// Represents an error that occurred during the processing of a command or query.
        /// </summary>
        public IError? Error { get; set; }
    }
}
