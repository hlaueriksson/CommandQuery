using System;
using System.Text.Json;

namespace CommandQuery.SystemTextJson
{
    internal static class JsonExtensions
    {
        internal static object? SafeDeserialize(this string json, Type type)
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
    }
}
