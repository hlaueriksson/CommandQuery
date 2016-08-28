using System;
using Newtonsoft.Json.Linq;

namespace CommandQuery.Internal
{
    internal static class JsonExtensions
    {
        public static object SafeToObject(this JObject json, Type type)
        {
            try
            {
                return json.ToObject(type);
            }
            catch
            {
                return null;
            }
        }
    }
}