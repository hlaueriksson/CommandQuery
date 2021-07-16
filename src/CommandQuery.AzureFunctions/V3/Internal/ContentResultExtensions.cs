#if NETCOREAPP3_1
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CommandQuery.AzureFunctions
{
    internal static class ContentResultExtensions
    {
        internal static ContentResult Ok(this object? result, JsonSerializerSettings? settings)
        {
            return new()
            {
                ContentType = "application/json; charset=utf-8",
                StatusCode = StatusCodes.Status200OK,
                Content = JsonConvert.SerializeObject(result, settings),
            };
        }

        internal static ContentResult BadRequest(this Exception exception, JsonSerializerSettings? settings)
        {
            return new()
            {
                ContentType = "application/json; charset=utf-8",
                StatusCode = StatusCodes.Status400BadRequest,
                Content = JsonConvert.SerializeObject(exception.ToError(), settings),
            };
        }

        internal static ContentResult InternalServerError(this Exception exception, JsonSerializerSettings? settings)
        {
            return new()
            {
                ContentType = "application/json; charset=utf-8",
                StatusCode = StatusCodes.Status500InternalServerError,
                Content = JsonConvert.SerializeObject(exception.ToError(), settings),
            };
        }
    }
}
#endif
