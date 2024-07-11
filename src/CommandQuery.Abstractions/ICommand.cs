namespace CommandQuery
{
    /// <summary>
    /// Marker interface to represent a command.
    /// </summary>
    public interface ICommand;

    /// <summary>
    /// Marker interface to represent a command with result.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    public interface ICommand<TResult>;
}
