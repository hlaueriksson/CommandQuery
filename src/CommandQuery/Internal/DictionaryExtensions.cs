using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CommandQuery.Specs")]

namespace CommandQuery.Internal
{
    internal static class DictionaryExtensions
    {
        public static object SafeToObject(this IDictionary<string, string> dictionary, Type type)
        {
            try
            {
                var result = Activator.CreateInstance(type);

                foreach (var kv in dictionary)
                {
                    var property = type.GetProperty(kv.Key);

                    if (property == null) continue;

                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    property.SetValue(result, Convert.ChangeType(kv.Value, propertyType));
                }

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}