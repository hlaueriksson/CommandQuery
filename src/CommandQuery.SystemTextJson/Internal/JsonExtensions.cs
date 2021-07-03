using System;
using System.Text.Json;

namespace CommandQuery.SystemTextJson.Internal
{
    internal static class JsonExtensions
    {
        public static object? SafeToObject(this string json, Type type)
        {
            try
            {
                return JsonSerializer.Deserialize(json, type);
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
                return JsonSerializer.Serialize(payload);
            }
            catch
            {
                return null;
            }
        }
    }
}
