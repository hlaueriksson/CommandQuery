using CommandQuery.Exceptions;

namespace CommandQuery.Sample.AspNet.WebApi.Handlers
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
