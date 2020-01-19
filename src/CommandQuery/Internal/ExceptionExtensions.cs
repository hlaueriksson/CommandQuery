using System;
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
                case CommandProcessorException _:
                    return 1001;
                case CommandException _:
                    return 1002;
                default:
                    return 1003;
            }
        }

        public static int GetQueryEventId(this Exception exception)
        {
            switch (exception)
            {
                case QueryProcessorException _:
                    return 2001;
                case QueryException _:
                    return 2002;
                default:
                    return 2003;
            }
        }

        public static string GetCommandCategory(this Exception exception)
        {
            switch (exception)
            {
                case CommandProcessorException _:
                case CommandException _:
                    return exception.GetType().Name;
                default:
                    return "UnhandledCommandException";
            }
        }

        public static string GetQueryCategory(this Exception exception)
        {
            switch (exception)
            {
                case QueryProcessorException _:
                case QueryException _:
                    return exception.GetType().Name;
                default:
                    return "UnhandledQueryException";
            }
        }
    }
}