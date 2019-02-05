using System;

namespace CommandQuery.Exceptions
{
    /// <summary>
    /// Represents errors that occur during command processing.
    /// </summary>
    public class CommandProcessorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProcessorException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommandProcessorException(string message) : base(message)
        {
        }
    }
}