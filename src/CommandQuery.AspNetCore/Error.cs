namespace CommandQuery.AspNetCore
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
    }
}