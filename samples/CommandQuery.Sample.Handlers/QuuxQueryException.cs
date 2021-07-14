using CommandQuery.Exceptions;

namespace CommandQuery.Sample.Handlers
{
    public class QuuxQueryException : QueryException
    {
        public bool InvalidCorge { get; set; }
        public bool InvalidGrault { get; set; }

        public QuuxQueryException(string message) : base(message)
        {
        }
    }
}
