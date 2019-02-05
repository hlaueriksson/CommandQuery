namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// EventIds for logging.
    /// </summary>
    public static class LogEvents
    {
        /// <summary><see cref="Exceptions.CommandProcessorException" /></summary>
        public const int CommandProcessorException = 1001;
        /// <summary><see cref="Exceptions.CommandValidationException" /></summary>
        public const int CommandValidationException = 1002;
        /// <summary><see cref="System.Exception" /></summary>
        public const int CommandException = 1003;

        /// <summary><see cref="Exceptions.QueryProcessorException" /></summary>
        public const int QueryProcessorException = 2001;
        /// <summary><see cref="Exceptions.QueryValidationException" /></summary>
        public const int QueryValidationException = 2002;
        /// <summary><see cref="System.Exception" /></summary>
        public const int QueryException = 2003;
    }
}