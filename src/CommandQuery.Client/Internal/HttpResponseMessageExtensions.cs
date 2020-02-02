using System.Net.Http;

namespace CommandQuery.Client.Internal
{
    internal static class HttpResponseMessageExtensions
    {
        public static HttpResponseMessage EnsureSuccess(this HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
            {
                var error = message.Content.ReadAsAsync<Error>()
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                throw new CommandQueryException(message.ToString(), error);
            }

            return message;
        }
    }
}