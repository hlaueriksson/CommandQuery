using System.Text.Json;
using System.Text.Json.Serialization;

namespace CommandQuery.SystemTextJson
{
    internal static class JsonExtensions
    {
        private static readonly JsonSerializerOptions _options = GetJsonSerializerOptions();

        internal static object? SafeDeserialize(this string json, Type type, JsonSerializerOptions? options = null)
        {
            try
            {
                return JsonSerializer.Deserialize(json, type, options ?? _options);
            }
            catch
            {
                return null;
            }
        }

        internal static object? SafeDeserialize(this IDictionary<string, object>? dictionary, Type type)
        {
            if (dictionary is null)
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize(JsonSerializer.Serialize(dictionary), type, _options);
            }
            catch
            {
                return null;
            }
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var result = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };
            result.Converters.Add(new JsonStringEnumConverter());
            result.Converters.Add(new BooleanConverter());

            return result;
        }
    }
}
