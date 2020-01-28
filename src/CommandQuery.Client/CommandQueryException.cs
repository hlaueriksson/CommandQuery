using System;

namespace CommandQuery.Client
{
    public class CommandQueryException : Exception
    {
        public Error Error { get; }

        public CommandQueryException(string message, Error error) : base(message)
        {
            Error = error;
        }
    }
}