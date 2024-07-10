using System.Text;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.AzureFunctions
{
    internal static class HttpRequestExtensions
    {
        internal static async Task<string?> ReadAsStringAsync(this HttpRequest req, Encoding? encoding = null)
        {
            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            if (req.Body is null)
            {
                return null;
            }

            using (var reader = new StreamReader(req.Body, bufferSize: 1024, detectEncodingFromByteOrderMarks: true, encoding: encoding ?? Encoding.UTF8, leaveOpen: true))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}
