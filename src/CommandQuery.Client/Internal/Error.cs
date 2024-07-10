using System.Collections.Generic;

namespace CommandQuery.Client
{
    internal class Error : IError
    {
        public string? Message { get; set; }

        public Dictionary<string, object>? Details { get; set; }
    }
}
