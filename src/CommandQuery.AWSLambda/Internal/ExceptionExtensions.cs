using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.Internal;

namespace CommandQuery.AWSLambda.Internal
{
    internal static class ExceptionExtensions
    {
        public static APIGatewayProxyResponse ToBadRequest(this Exception exception)
        {
            return new()
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonSerializer.Serialize(exception.ToError()),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
            };
        }

        public static APIGatewayProxyResponse ToInternalServerError(this Exception exception)
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
