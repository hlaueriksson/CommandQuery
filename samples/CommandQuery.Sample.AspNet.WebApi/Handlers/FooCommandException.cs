using CommandQuery.Exceptions;

namespace CommandQuery.Sample.AspNet.WebApi.Handlers
{
    public class FooCommandException : CommandException
    {
        public int ErrorCode { get; }
        public string Help { get; }

        public FooCommandException(string message, int errorCode, string help) : base(message)
        {
            ErrorCode = errorCode;
            Help = help;
        }
    }
}