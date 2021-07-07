using System.IO;
using System.Text;
using System.Text.Json;
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

        internal static async Task OkAsync(this HttpResponse response, object? result)
        {
            response.StatusCode = StatusCodes.Status200OK;
            response.ContentType = "text/json";
            await JsonSerializer.SerializeAsync(response.Body, result).ConfigureAwait(false);
        }

        internal static async Task BadRequestAsync(this HttpResponse response, IError error)
        {
            response.StatusCode = StatusCodes.Status400BadRequest;
            response.ContentType = "text/json";
            await JsonSerializer.SerializeAsync(response.Body, error).ConfigureAwait(false);
        }

        internal static async Task InternalServerErrorAsync(this HttpResponse response, IError error)
        {
            response.StatusCode = StatusCodes.Status500InternalServerError;
            response.ContentType = "text/json";
            await JsonSerializer.SerializeAsync(response.Body, error).ConfigureAwait(false);
        }
    }
}
