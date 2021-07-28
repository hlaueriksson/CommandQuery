using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandQuery.Exceptions;

namespace CommandQuery
{
    internal static class ExceptionExtensions
    {
        internal static bool IsHandled(this Exception exception)
        {
            return
                exception is CommandProcessorException ||
                exception is CommandException ||
                exception is QueryProcessorException ||
                exception is QueryException;
        }

        internal static IError ToError(this Exception exception)
        {
            return exception switch
            {
                CommandException commandException => commandException.ToError(),
                QueryException queryException => queryException.ToError(),
                _ => new Error(exception.Message),
            };
        }

        internal static IError ToError(this CommandException exception)
        {
            return new Error(exception.Message, GetDetails(exception));
        }

        internal static IError ToError(this QueryException exception)
        {
            return new Error(exception.Message, GetDetails(exception));
        }

        private static Dictionary<string, object>? GetDetails(Exception exception)
        {
            var properties = exception.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.DeclaringType != typeof(Exception))
                .Where(x => x.GetValue(exception) != null)
                .ToList();

            return properties.Any() ? properties.ToDictionary(property => property.Name, property => property.GetValue(exception)) : null;
        }
    }
}
