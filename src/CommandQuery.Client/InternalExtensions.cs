using System.Linq;

namespace CommandQuery.Client
{
    internal static class InternalExtensions
    {
        internal static string GetRequestUri(this object value)
        {
            return value.GetType().Name + "?" + value.QueryString();
        }

        private static string QueryString(this object value)
        {
            var properties = from p in value.GetType().GetProperties()
                             where p.GetValue(value, null) != null
                             select p.Name + "=" + System.Net.WebUtility.UrlEncode(p.GetValue(value, null).ToString());

            return string.Join("&", properties.ToArray());
        }
    }
}