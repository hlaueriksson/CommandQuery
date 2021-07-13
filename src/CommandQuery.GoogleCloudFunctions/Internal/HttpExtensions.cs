using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.GoogleCloudFunctions
{
    internal static class HttpExtensions
    {
        private const int DefaultBufferSize = 1024;

        // https://github.com/Azure/azure-webjobs-sdk-extensions/blob/main/src/WebJobs.Extensions.Http/Extensions/HttpRequestExtensions.cs#L26
        internal static async Task<string> ReadAsStringAsync(this HttpRequest request)
        {
            request.EnableBuffering();

            string? result = null;
            using (var reader = new StreamReader(
                request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                bufferSize: DefaultBufferSize,
                leaveOpen: true))
            {
                result = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            request.Body.Seek(0, SeekOrigin.Begin);

            return result;
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
