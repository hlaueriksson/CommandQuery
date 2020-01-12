using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandQuery.Exceptions;

namespace CommandQuery.Internal
{
    internal static class ExceptionExtensions
    {
        public static Error ToError(this Exception exception)
        {
            return new Error { Message = exception.Message };
        }

        public static Error ToError(this CommandException exception)
        {
            return new Error { Message = exception.Message, Details = GetDetails(exception) };
        }

        public static Error ToError(this QueryException exception)
        {
            return new Error { Message = exception.Message, Details = GetDetails(exception) };
        }

        private static Dictionary<string, object> GetDetails(Exception exception)
        {
            var properties = exception.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.DeclaringType != typeof(Exception))
                .Where(x => x.GetValue(exception) != null)
                .ToList();

            return properties.Any() ? properties.ToDictionary(property => property.Name, property => property.GetValue(exception)) : null;
        }
    }
}