using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.GoogleCloudFunctions
{
    internal static class HttpExtensions
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

            using (var reader = new StreamReader(req.Body, encoding: encoding ?? Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        internal static async Task OkAsync(this HttpResponse response, object? result, JsonSerializerOptions? options, CancellationToken cancellationToken)
        {
            response.ContentType = "application/json; charset=utf-8";
            response.StatusCode = StatusCodes.Status200OK;
            await JsonSerializer.SerializeAsync(response.Body, result, options, cancellationToken).ConfigureAwait(false);
        }

        internal static async Task BadRequestAsync(this HttpResponse response, Exception exception, JsonSerializerOptions? options, CancellationToken cancellationToken)
        {
            response.ContentType = "application/json; charset=utf-8";
            response.StatusCode = StatusCodes.Status400BadRequest;
            await JsonSerializer.SerializeAsync(response.Body, exception.ToError(), options, cancellationToken).ConfigureAwait(false);
        }

        internal static async Task InternalServerErrorAsync(this HttpResponse response, Exception exception, JsonSerializerOptions? options, CancellationToken cancellationToken)
        {
            response.ContentType = "application/json; charset=utf-8";
            response.StatusCode = StatusCodes.Status500InternalServerError;
            await JsonSerializer.SerializeAsync(response.Body, exception.ToError(), options, cancellationToken).ConfigureAwait(false);
        }
    }
}
