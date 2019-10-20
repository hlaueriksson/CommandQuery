namespace CommandQuery
{
    /// <summary>
    /// Marker interface to represent a command.
    /// </summary>
    public interface ICommand
    {
    }

    public interface ICommand<TResult>
    {
    }
}