using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CommandQuery.Internal
{
    internal static class HelpExtensions
    {
        public static string GetCurl(this Type type, string baseUrl)
        {
            var slug = type.Name;
            var json = GetJson(type);

            return $"curl -X POST -d \"{json}\" {baseUrl}/{slug} --header \"Content-Type:application/json\"";
        }

        private static string GetJson(this Type query)
        {
            var instance = Activator.CreateInstance(query);

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.QuoteChar = '\'';

                var serializer = new JsonSerializer();
                serializer.Serialize(writer, instance);
            }

            return sb.ToString();
        }
    }
}