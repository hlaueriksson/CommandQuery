using System;
using Newtonsoft.Json;

namespace CommandQuery.NewtonsoftJson
{
    internal static class JsonExtensions
    {
        internal static object? SafeToObject(this string json, Type type)
        {
            try
            {
                return JsonConvert.DeserializeObject(json, type);
            }
            catch
            {
                return null;
            }
        }
    }
}
