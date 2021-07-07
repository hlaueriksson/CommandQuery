using System.Net.Http;
using System.Threading.Tasks;

namespace CommandQuery.Client
{
    internal static class HttpResponseMessageExtensions
    {
        internal static async Task<HttpResponseMessage> EnsureSuccessAsync(this HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                return message;
            }

            var error = await message.Content.ReadAsAsync<Error>().ConfigureAwait(false);

            throw new CommandQueryException(message.ToString(), error);
        }
    }
}
