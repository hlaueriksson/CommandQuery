using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommandQuery.Internal
{
    internal static class DictionaryExtensions
    {
        public static object SafeToObject(this IDictionary<string, JToken> dictionary, Type type)
        {
            if (dictionary == null) return null;

            try
            {
                return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dictionary), type);
            }
            catch
            {
                return null;
            }
        }
    }
}