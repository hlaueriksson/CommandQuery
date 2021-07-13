using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommandQuery.NewtonsoftJson
{
    internal static class DictionaryExtensions
    {
        internal static object? SafeDeserialize(this IDictionary<string, object>? dictionary, Type type)
        {
            if (dictionary is null)
            {
                return null;
            }

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
