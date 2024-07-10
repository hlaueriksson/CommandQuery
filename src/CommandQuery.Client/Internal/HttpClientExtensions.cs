using System.Net.Http.Headers;
using System.Reflection;

namespace CommandQuery.Client
{
    internal static class HttpClientExtensions
    {
        internal static void SetDefaultRequestHeaders(this HttpClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", $"CommandQuery.Client/{Assembly.GetExecutingAssembly().GetName().Version}");
        }
    }
}
