using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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

        private static readonly Assembly _system = typeof(object).Assembly;

        private static string QueryString(this object query)
        {
            var result = new List<string>();

            Parameters(query, string.Empty);

            return string.Join("&", result.ToArray());

            void Parameters(object root, string prefix)
            {
                foreach (var p in root.GetType().GetProperties().Where(p => p.GetValue(root, null) != null))
                {
                    var value = p.GetValue(root, null);

                    if (value.GetType().Assembly != _system)
                    {
                        Parameters(value, prefix + p.Name + ".");
                    }
                    else if (value is IEnumerable enumerable and not string)
                    {
                        result.AddRange(from object v in enumerable select Parameter(p, v, prefix));
                    }
                    else
                    {
                        result.Add(Parameter(p, value, prefix));
                    }
                }
            }

            static string Parameter(PropertyInfo property, object value, string prefix)
            {
                return value switch
                {
                    DateTime dateTime => NameValuePair(dateTime.ToString("O")),
                    DateTimeOffset dateTimeOffset => NameValuePair(dateTimeOffset.ToString("O")),
                    _ => NameValuePair(Convert.ToString(value, CultureInfo.InvariantCulture)!)
                };

                string NameValuePair(string value)
                {
                    return $"{prefix}{property.Name}={WebUtility.UrlEncode(value)}";
                }
            }
        }
    }
}
