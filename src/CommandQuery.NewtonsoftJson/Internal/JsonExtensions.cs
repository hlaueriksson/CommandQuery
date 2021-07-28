using System;
using Newtonsoft.Json;

namespace CommandQuery.NewtonsoftJson
{
    internal static class JsonExtensions
    {
        internal static object? SafeDeserialize(this string json, Type type, JsonSerializerSettings? settings)
        {
            try
            {
                return JsonConvert.DeserializeObject(json, type, settings);
            }
            catch
            {
                return null;
            }
        }
    }
}
