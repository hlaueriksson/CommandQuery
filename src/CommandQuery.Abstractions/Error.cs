using System.Collections.Generic;

namespace CommandQuery
{
    /// <summary>
    /// Represents an error that occurred during the processing of a command or query.
    /// </summary>
#pragma warning disable CA1716 // Identifiers should not match keywords
    public class Error // TODO: IError?
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        /// <summary>
        /// A message that describes the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Details about the error.
        /// </summary>
        public Dictionary<string, object> Details { get; set; }
    }
}
