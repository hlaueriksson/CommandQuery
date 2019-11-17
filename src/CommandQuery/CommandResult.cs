namespace CommandQuery
{
    /// <summary>
    /// Wraps a command result.
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Represents the result of an <see cref="ICommand"/>.
        /// </summary>
        public static readonly CommandResult None = new CommandResult();

        /// <summary>
        /// The result of an <see cref="ICommand&lt;TResult&gt;"/>.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult" /> class that wraps a command result.
        /// </summary>
        /// <param name="value">The result of an <see cref="ICommand&lt;TResult&gt;"/></param>
        public CommandResult(object value)
        {
            Value = value;
        }

        private CommandResult()
        {
        }
    }
}