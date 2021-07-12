#if NET5_0
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.AzureFunctions
{
    internal static class HttpRequestDataExtensions
    {
        internal static async Task<HttpResponseData> OkAsync(this HttpRequestData req, object? result)
        {
            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(result).ConfigureAwait(false);
            return response;
        }

        internal static async Task<HttpResponseData> BadRequestAsync(this HttpRequestData req, Exception exception)
        {
            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(exception.ToError()).ConfigureAwait(false);
            response.StatusCode = HttpStatusCode.BadRequest;
            return response;
        }

        internal static async Task<HttpResponseData> InternalServerErrorAsync(this HttpRequestData req, Exception exception)
        {
            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(exception.ToError()).ConfigureAwait(false);
            response.StatusCode = HttpStatusCode.InternalServerError;
            return response;
        }
    }
}
#endif
