using System;

namespace CommandQuery.Exceptions
{
    /// <summary>
    /// Represents errors that occur during command processing in a command handler.
    /// Derive custom command exceptions from this class and add public properties to expose more error details.
    /// </summary>
    public class CommandException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CommandException(string message) : base(message)
        {
        }
    }
}