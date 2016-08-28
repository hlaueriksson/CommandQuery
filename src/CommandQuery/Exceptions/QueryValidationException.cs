using System;

namespace CommandQuery.Exceptions
{
    public class QueryValidationException : Exception
    {
        public QueryValidationException(string message) : base(message)
        {
        }
    }
}