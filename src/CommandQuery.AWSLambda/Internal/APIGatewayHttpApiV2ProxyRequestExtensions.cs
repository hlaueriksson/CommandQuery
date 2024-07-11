using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace CommandQuery.AWSLambda
{
    internal static class APIGatewayHttpApiV2ProxyRequestExtensions
    {
        internal static APIGatewayHttpApiV2ProxyResponse Ok(this APIGatewayHttpApiV2ProxyRequest request, object? result, JsonSerializerOptions? options = null)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(result, options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
            };
        }

        internal static APIGatewayHttpApiV2ProxyResponse BadRequest(this APIGatewayHttpApiV2ProxyRequest request, Exception exception, JsonSerializerOptions? options = null)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonSerializer.Serialize(exception.ToError(), options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
            };
        }

        internal static APIGatewayHttpApiV2ProxyResponse InternalServerError(this APIGatewayHttpApiV2ProxyRequest request, Exception exception, JsonSerializerOptions? options = null)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = JsonSerializer.Serialize(exception.ToError(), options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
            };
        }
    }
}
