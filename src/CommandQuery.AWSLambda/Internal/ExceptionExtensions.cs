using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace CommandQuery.AWSLambda
{
    internal static class ExceptionExtensions
    {
        internal static APIGatewayProxyResponse ToBadRequest(this Exception exception)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonSerializer.Serialize(exception.ToError()),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
            };
        }

        internal static APIGatewayProxyResponse ToInternalServerError(this Exception exception)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = JsonSerializer.Serialize(exception.ToError()),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
            };
        }
    }
}
