using System.Text;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.AzureFunctions
{
    internal static class HttpRequestExtensions
    {
        internal static async Task<string?> ReadAsStringAsync(this HttpRequest req, Encoding? encoding = null)
        {
            ArgumentNullException.ThrowIfNull(req);

            if (req.Body is null)
            {
                return null;
            }

            using (var reader = new StreamReader(req.Body, encoding: encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}
