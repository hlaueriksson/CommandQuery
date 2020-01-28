using System.Collections.Generic;
using CommandQuery.Exceptions;

namespace CommandQuery
{
    /// <summary>
    /// Represents an error that occured during the processing of a command or query.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// A message that describes the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Details about the error.
        /// The details are taken from public properties in custom exceptions derived from <see cref="CommandException" /> or <see cref="QueryException" />.
        /// </summary>
        public Dictionary<string, object> Details { get; set; }
    }
}