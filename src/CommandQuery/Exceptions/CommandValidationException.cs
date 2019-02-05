using System;

namespace CommandQuery.Exceptions
{
    /// <summary>
    /// Represents errors that occur during command validation.
    /// </summary>
    public class CommandValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidationException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommandValidationException(string message) : base(message)
        {
        }
    }
}