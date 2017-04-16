using System;

namespace CommandQuery.Exceptions
{
    public class CommandProcessorException : Exception
    {
        public CommandProcessorException(string message) : base(message)
        {
        }
    }
}