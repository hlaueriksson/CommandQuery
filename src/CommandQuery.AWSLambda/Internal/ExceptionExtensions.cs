using System;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace CommandQuery.AWSLambda.Internal
{
    internal static class ExceptionExtensions
    {
        public static Error ToError(this Exception exception)
        {
            return new Error { Message = exception.Message };
        }

        public static APIGatewayProxyResponse ToBadRequest(this Exception exception)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = JsonConvert.SerializeObject(exception.ToError()),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        public static APIGatewayProxyResponse ToInternalServerError(this Exception exception)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Body = JsonConvert.SerializeObject(exception.ToError()),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}