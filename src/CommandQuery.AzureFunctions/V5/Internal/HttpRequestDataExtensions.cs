#if NET5_0
using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.AzureFunctions
{
    internal static class HttpRequestDataExtensions
    {
        internal static async Task<HttpResponseData> OkAsync(this HttpRequestData req, object? result, JsonSerializerOptions? options, CancellationToken cancellationToken)
        {
            var response = req.CreateResponse();
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.StatusCode = HttpStatusCode.OK;
            await JsonSerializer.SerializeAsync(response.Body, result, options, cancellationToken).ConfigureAwait(false);
            return response;
        }

        internal static async Task<HttpResponseData> BadRequestAsync(this HttpRequestData req, Exception exception, JsonSerializerOptions? options, CancellationToken cancellationToken)
        {
            var response = req.CreateResponse();
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.StatusCode = HttpStatusCode.BadRequest;
            await JsonSerializer.SerializeAsync(response.Body, exception.ToError(), options, cancellationToken).ConfigureAwait(false);
            return response;
        }

        internal static async Task<HttpResponseData> InternalServerErrorAsync(this HttpRequestData req, Exception exception, JsonSerializerOptions? options, CancellationToken cancellationToken)
        {
            var response = req.CreateResponse();
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.StatusCode = HttpStatusCode.InternalServerError;
            await JsonSerializer.SerializeAsync(response.Body, exception.ToError(), options, cancellationToken).ConfigureAwait(false);
            return response;
        }
    }
}
#endif
