using System;
using System.Runtime.CompilerServices;

[assembly:
    InternalsVisibleTo("CommandQuery.AspNet.WebApi"),
    InternalsVisibleTo("CommandQuery.AspNetCore"),
    InternalsVisibleTo("CommandQuery.AWSLambda"),
    InternalsVisibleTo("CommandQuery.AzureFunctions")]

namespace CommandQuery.Internal
{
    internal static class ExceptionExtensions
    {
        public static Error ToError(this Exception exception)
        {
            return new Error { Message = exception.Message };
        }
    }
}