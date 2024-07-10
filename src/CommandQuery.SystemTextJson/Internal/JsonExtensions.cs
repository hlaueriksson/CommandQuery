using System.Text.Json;

namespace CommandQuery.SystemTextJson
{
    internal static class JsonExtensions
    {
        internal static object? SafeDeserialize(this string json, Type type, JsonSerializerOptions? options)
        {
            try
            {
                return JsonSerializer.Deserialize(json, type, options);
            }
            catch
            {
                return null;
            }
        }
    }
}
