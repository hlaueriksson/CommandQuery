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

            var result = query.ToDictionary(g => g.Key, Token, StringComparer.OrdinalIgnoreCase);

            var nestedKeys = result.Keys.Where(x => x.Contains('.')).ToList();

            foreach (var key in nestedKeys)
            {
                var path = key.Split('.');
                var ancestorCount = key.Count(x => x == '.');

                Dictionary<string, object> parent = result;

                foreach (var ancestor in path.Take(ancestorCount))
                {
                    if (parent!.ContainsKey(ancestor))
                    {
                        parent = (Dictionary<string, object>)parent[ancestor];
                    }
                    else
                    {
                        var temp = new Dictionary<string, object>();
                        parent.Add(ancestor, temp);
                        parent = temp;
                    }
                }

                parent.Add(path.Last(), result[key]);
                result.Remove(key);
            }

            return result;

            object Token(KeyValuePair<string, IEnumerable<string>> kv)
            {
                var property = properties.FirstOrDefault(x => string.Equals(x.Name, kv.Key, StringComparison.OrdinalIgnoreCase));
                var isEnumerable = property?.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(property?.PropertyType);

                return (isEnumerable ? kv.Value : kv.Value.FirstOrDefault())!;
            }
        }
    }
}
