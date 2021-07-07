using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandQuery.Exceptions;

namespace CommandQuery.Internal
{
    internal static class ExceptionExtensions
    {
        public static bool IsHandled(this Exception exception)
        {
            return
                exception is CommandProcessorException ||
                exception is CommandException ||
                exception is QueryProcessorException ||
                exception is QueryException;
        }

        public static IError ToError(this Exception exception)
        {
            switch (exception)
            {
                case CommandException commandException:
                    return commandException.ToError();
                case QueryException queryException:
                    return queryException.ToError();
                default:
                    return new Error(exception.Message);
            }
        }

        public static IError ToError(this CommandException exception)
        {
            return new Error(exception.Message, GetDetails(exception));
        }

        public static IError ToError(this QueryException exception)
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
