using System;

namespace CommandQuery.Client
{
    /// <summary>
    /// Represents errors that occur when receiving HTTP responses in the <see cref="CommandClient" /> and <see cref="QueryClient" />.
    /// </summary>
    public class CommandQueryException : Exception
    {
        /// <summary>
        /// Represents an error that occured during the processing of a command or query.
        /// </summary>
        public Error Error { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandQueryException" /> class with a specified error message and <see cref="CommandQuery.Error" />.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error"></param>
        public CommandQueryException(string message, Error error) : base(message)
        {
            Error = error;
        }
    }
}