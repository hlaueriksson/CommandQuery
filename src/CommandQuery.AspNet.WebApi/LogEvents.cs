namespace CommandQuery.AspNet.WebApi
{
    /// <summary>
    /// Categories for logging.
    /// </summary>
    public static class LogEvents
    {
        /// <summary><see cref="Exceptions.CommandProcessorException" /></summary>
        public const string CommandProcessorException = "CommandProcessorException";
        /// <summary><see cref="Exceptions.CommandException" /></summary>
        public const string CommandException = "CommandException";
        /// <summary><see cref="System.Exception" /></summary>
        public const string UnhandledCommandException = "UnhandledCommandException";

        /// <summary><see cref="Exceptions.QueryProcessorException" /></summary>
        public const string QueryProcessorException = "QueryProcessorException";
        /// <summary><see cref="Exceptions.QueryValidationException" /></summary>
        public const string QueryValidationException = "QueryValidationException";
        /// <summary><see cref="System.Exception" /></summary>
        public const string QueryException = "QueryException";
    }
}