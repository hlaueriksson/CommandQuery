using System;
using Newtonsoft.Json;

namespace CommandQuery.NewtonsoftJson.Internal
{
    internal static class JsonExtensions
    {
        public static object? SafeToObject(this string json, Type type)
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

        public static string? ToJson(this object payload)
        {
            try
            {
                return JsonConvert.SerializeObject(payload);
            }
            catch
            {
                return null;
            }
        }
    }
}
