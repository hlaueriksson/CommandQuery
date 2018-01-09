using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CommandQuery.Specs")]

namespace CommandQuery.AspNetCore.Internal
{
    internal class Error
    {
        public string Message { get; set; }
    }

    internal static class ExceptionExtensions
    {
        public static Error ToError(this Exception exception)
        {
            return new Error { Message = exception.Message };
        }
    }
}