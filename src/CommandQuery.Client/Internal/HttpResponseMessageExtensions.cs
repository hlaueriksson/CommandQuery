using System.Net.Http.Json;
using System.Text.Json;

namespace CommandQuery.Client
{
    internal static class HttpResponseMessageExtensions
    {
        private static readonly JsonSerializerOptions _options = GetJsonSerializerOptions();

        internal static async Task<HttpResponseMessage> EnsureSuccessAsync(this HttpResponseMessage message, CancellationToken cancellationToken)
        {
            if (message.IsSuccessStatusCode)
            {
                return message;
            }

            var error = await message.Content.ReadFromJsonAsync<Error>(_options, cancellationToken).ConfigureAwait(false);

            throw new CommandQueryException(message.ToString(), error);
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var result = new JsonSerializerOptions();
            result.Converters.Add(new DictionaryStringObjectConverter());

            return result;
        }
    }
}
