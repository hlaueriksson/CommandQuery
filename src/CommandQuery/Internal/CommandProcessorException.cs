using System;

namespace CommandQuery.Internal
{
    internal class CommandProcessorException : Exception
    {
        public CommandProcessorException(string message) : base(message)
        {
        }
    }
}