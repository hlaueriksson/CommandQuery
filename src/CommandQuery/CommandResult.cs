namespace CommandQuery
{
    public class CommandResult
    {
        public static readonly CommandResult None = new CommandResult();

        public object Value { get; }

        public CommandResult(object value)
        {
            Value = value;
        }

        private CommandResult()
        {
        }
    }
}