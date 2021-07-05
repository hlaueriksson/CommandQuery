using System.Collections.Generic;

namespace CommandQuery.Internal
{
    internal class Error : IError
    {
        public Error(string message)
        {
            Message = message;
        }

        public Error(string message, Dictionary<string, object>? details)
        {
            Message = message;
            Details = details;
        }

        public string? Message { get; }

        public Dictionary<string, object>? Details { get; }
    }
}
