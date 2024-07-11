using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace CommandQuery.AWSLambda
{
    internal static class APIGatewayProxyRequestExtensions
    {
        internal static APIGatewayProxyResponse Ok(this APIGatewayProxyRequest request, object? result, JsonSerializerOptions? options = null)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(result, options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
            };
        }

        internal static APIGatewayProxyResponse BadRequest(this APIGatewayProxyRequest request, Exception exception, JsonSerializerOptions? options = null)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonSerializer.Serialize(exception.ToError(), options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
            };
        }

        internal static APIGatewayProxyResponse InternalServerError(this APIGatewayProxyRequest request, Exception exception, JsonSerializerOptions? options = null)
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
