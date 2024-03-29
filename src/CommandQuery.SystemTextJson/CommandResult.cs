namespace CommandQuery.SystemTextJson
{
    /// <summary>
    /// Wraps a command result.
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Represents the result of an <see cref="ICommand"/>.
        /// </summary>
        public static readonly CommandResult None = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult"/> class that wraps a command result.
        /// </summary>
        /// <param name="value">The result of an <see cref="ICommand{TResult}"/>.</param>
        public CommandResult(object value)
        {
            Value = value;
        }

        private CommandResult()
        {
        }

        /// <summary>
        /// The result of an <see cref="ICommand{TResult}"/>.
        /// </summary>
        public object? Value { get; }
    }
}
