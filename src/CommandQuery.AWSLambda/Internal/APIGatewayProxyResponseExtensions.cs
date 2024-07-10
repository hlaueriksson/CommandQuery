using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace CommandQuery.AWSLambda
{
    internal static class APIGatewayProxyResponseExtensions
    {
        internal static APIGatewayProxyResponse Ok(this object? result, JsonSerializerOptions? options)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonSerializer.Serialize(result, options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
            };
        }

        internal static APIGatewayProxyResponse BadRequest(this Exception exception, JsonSerializerOptions? options)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonSerializer.Serialize(exception.ToError(), options),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
            };
        }

        internal static APIGatewayProxyResponse InternalServerError(this Exception exception, JsonSerializerOptions? options)
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
