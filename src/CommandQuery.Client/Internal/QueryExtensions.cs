using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandQuery.Client
{
    internal static class QueryExtensions
    {
        internal static string GetRequestUri(this object query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.GetType().Name + "?" + query.QueryString();
        }

        internal static string GetRequestSlug(this object query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.GetType().Name;
        }

        private static string QueryString(this object query)
        {
            var result = new List<string>();

            foreach (var p in query.GetType().GetProperties().Where(p => p.GetValue(query, null) != null))
            {
                var value = p.GetValue(query, null);

                if (!(value is string) && value is IEnumerable enumerable)
                {
                    result.AddRange(from object v in enumerable select Parameter(p, v));
                }
                else
                {
                    result.Add(Parameter(p, value));
                }
            }

            return string.Join("&", result.ToArray());

            string Parameter(PropertyInfo property, object value)
            {
                return $"{property.Name}={System.Net.WebUtility.UrlEncode(value.ToString())}";
            }
        }
    }
}
