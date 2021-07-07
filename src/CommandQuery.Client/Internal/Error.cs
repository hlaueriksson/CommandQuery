using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommandQuery.Client
{
    [JsonObject]
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
