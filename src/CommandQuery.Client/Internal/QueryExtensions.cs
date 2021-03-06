﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CommandQuery.Tests")]

namespace CommandQuery.Client.Internal
{
    internal static class QueryExtensions
    {
        internal static string GetRequestUri(this object query)
        {
            return query.GetType().Name + "?" + query.QueryString();
        }

        private static string QueryString(this object query)
        {
            var result = new List<string>();

            foreach (var p in query.GetType().GetProperties().Where(p => p.GetValue(query, null) != null))
            {
                var value = p.GetValue(query, null);

                if (!(value is string) && value is IEnumerable enumerable)
                    result.AddRange(from object v in enumerable select Parameter(p, v));
                else
                    result.Add(Parameter(p, value));
            }

            return string.Join("&", result.ToArray());

            string Parameter(PropertyInfo property, object value)
            {
                return $"{property.Name}={System.Net.WebUtility.UrlEncode(value.ToString())}";
            }
        }
    }
}