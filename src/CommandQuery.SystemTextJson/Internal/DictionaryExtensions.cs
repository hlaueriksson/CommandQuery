using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CommandQuery.SystemTextJson.Internal
{
    internal static class DictionaryExtensions
    {
        private static readonly JsonSerializerOptions _options = GetJsonSerializerOptions();

        public static object? SafeToObject(this IDictionary<string, object>? dictionary, Type type)
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
            result.Converters.Add(new BooleanConverter());

            return result;
        }
    }
}
