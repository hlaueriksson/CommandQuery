using System;

namespace CommandQuery.Internal
{
    internal class QueryProcessorException : Exception
    {
        public QueryProcessorException(string message) : base(message)
        {
        }
    }
}