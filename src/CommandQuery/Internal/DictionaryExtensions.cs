using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CommandQuery
{
    internal static class DictionaryExtensions
    {
        internal static Dictionary<string, object>? GetQueryDictionary(this IDictionary<string, IEnumerable<string>>? query, Type type)
        {
            if (query is null)
            {
                return null;
            }

            var properties = type.GetProperties();

            return query.ToDictionary(g => g.Key, Token, StringComparer.OrdinalIgnoreCase);

            object Token(KeyValuePair<string, IEnumerable<string>> kv)
            {
                var property = properties.FirstOrDefault(x => string.Equals(x.Name, kv.Key, StringComparison.OrdinalIgnoreCase));
                var isEnumerable = property?.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property?.PropertyType);

                return (isEnumerable ? kv.Value : kv.Value.FirstOrDefault())!;
            }
        }
    }
}
