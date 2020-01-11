namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// EventIds for logging.
    /// </summary>
    public static class LogEvents
    {
        /// <summary><see cref="Exceptions.CommandProcessorException" /></summary>
        public const int CommandProcessorException = 1001;
        /// <summary><see cref="Exceptions.CommandException" /></summary>
        public const int CommandException = 1002;
        /// <summary><see cref="System.Exception" /></summary>
        public const int UnhandledCommandException = 1003;

        /// <summary><see cref="Exceptions.QueryProcessorException" /></summary>
        public const int QueryProcessorException = 2001;
        /// <summary><see cref="Exceptions.QueryException" /></summary>
        public const int QueryException = 2002;
        /// <summary><see cref="System.Exception" /></summary>
        public const int UnhandledQueryException = 2003;
    }
}