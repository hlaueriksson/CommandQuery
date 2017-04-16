using System;

namespace CommandQuery.Exceptions
{
    public class QueryProcessorException : Exception
    {
        public QueryProcessorException(string message) : base(message)
        {
        }
    }
}