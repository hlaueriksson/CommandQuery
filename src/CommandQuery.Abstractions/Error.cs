using System.Collections.Generic;

namespace CommandQuery
{
    /// <summary>
    /// Represents an error that occurred during the processing of a command or query.
    /// </summary>
#pragma warning disable CA1716 // Identifiers should not match keywords
    public class Error // TODO: IError?
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public Error(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        /// <param name="details">Details about the error.</param>
        public Error(string message, Dictionary<string, object> details)
        {
            Message = message;
            Details = details;
        }

        /// <summary>
        /// A message that describes the error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Details about the error.
        /// </summary>
        public Dictionary<string, object>? Details { get; }
    }
}
