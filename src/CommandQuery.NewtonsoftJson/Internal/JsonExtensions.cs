using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommandQuery.NewtonsoftJson.Internal
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

        public static string ToJson(this object payload)
        {
            try
            {
                return JsonConvert.SerializeObject(payload);
            }
            catch
            {
                return null;
            }
        }
    }
}