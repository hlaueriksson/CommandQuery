using System.Collections.Generic;

namespace CommandQuery.Client.Internal
{
    internal class Error : IError
    {
        public string? Message { get; set; }

        public Dictionary<string, object>? Details { get; set; }
    }
}
