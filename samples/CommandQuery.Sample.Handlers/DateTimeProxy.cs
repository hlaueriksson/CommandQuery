namespace CommandQuery.Sample.Handlers
{
    public interface IDateTimeProxy
    {
        DateTime Now { get; }
    }

    public class DateTimeProxy : IDateTimeProxy
    {
        public DateTime Now => DateTime.Now;
    }
}