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

        public static int GetCommandEventId(this Exception exception)
        {
            switch (exception)
            {
                case CommandProcessorException:
                    return 1001;
                case CommandException:
                    return 1002;
                default:
                    return 1003;
            }
        }

        public static int GetQueryEventId(this Exception exception)
        {
            switch (exception)
            {
                case QueryProcessorException:
                    return 2001;
                case QueryException:
                    return 2002;
                default:
                    return 2003;
            }
        }

        public static string GetCommandCategory(this Exception exception)
        {
            switch (exception)
            {
                case CommandProcessorException:
                case CommandException:
                    return exception.GetType().Name;
                default:
                    return "UnhandledCommandException";
            }
        }

        public static string GetQueryCategory(this Exception exception)
        {
            switch (exception)
            {
                case QueryProcessorException:
                case QueryException:
                    return exception.GetType().Name;
                default:
                    return "UnhandledQueryException";
            }
        }

        public static Error ToError(this Exception exception)
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

        public static Error ToError(this CommandException exception)
        {
            return new(exception.Message, GetDetails(exception));
        }

        public static Error ToError(this QueryException exception)
        {
            return new(exception.Message, GetDetails(exception));
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
