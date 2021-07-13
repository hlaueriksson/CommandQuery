using System;
using System.Collections.Generic;

namespace CommandQuery.Client
{
    [Serializable]
    internal class Error : IError
    {
        public Error()
        {
        }

        public string? Message { get; set; }

        public Dictionary<string, object>? Details { get; set; }
    }
}
