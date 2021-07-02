using System;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using CommandQuery.Internal;
using Newtonsoft.Json;

namespace CommandQuery.AWSLambda.Internal
{
    internal static class ExceptionExtensions
    {
        public static APIGatewayProxyResponse ToBadRequest(this Exception exception)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonConvert.SerializeObject(exception.ToError()),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
            };
        }

        public static APIGatewayProxyResponse ToInternalServerError(this Exception exception)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = JsonConvert.SerializeObject(exception.ToError()),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
            };
        }
    }
}
