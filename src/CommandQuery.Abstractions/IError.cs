namespace CommandQuery
{
    /// <summary>
    /// Represents an error that occurred during the processing of a command or query.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// A message that describes the error.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Details about the error.
        /// </summary>
        public Dictionary<string, object>? Details { get; }
    }
}
