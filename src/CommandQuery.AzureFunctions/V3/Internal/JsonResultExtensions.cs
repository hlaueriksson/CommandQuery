#if NETCOREAPP3_1
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CommandQuery.AzureFunctions
{
    internal static class JsonResultExtensions
    {
        internal static JsonResult Ok(this object? result, JsonSerializerSettings? settings)
        {
            return settings != null ?
                new JsonResult(result, settings) { StatusCode = StatusCodes.Status200OK } :
                new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        internal static JsonResult BadRequest(this Exception exception, JsonSerializerSettings? settings)
        {
            return settings != null ?
                new JsonResult(exception.ToError(), settings) { StatusCode = StatusCodes.Status400BadRequest } :
                new JsonResult(exception.ToError()) { StatusCode = StatusCodes.Status400BadRequest };
        }

        internal static JsonResult InternalServerError(this Exception exception, JsonSerializerSettings? settings)
        {
            return settings != null ?
                new JsonResult(exception.ToError(), settings) { StatusCode = StatusCodes.Status500InternalServerError } :
                new JsonResult(exception.ToError()) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}
#endif
