namespace CommandQuery
{
    internal class Error : IError
    {
        internal Error(string message)
        {
            Message = message;
        }

        internal Error(string message, Dictionary<string, object>? details)
        {
            Message = message;
            Details = details;
        }

        public string? Message { get; }

        public Dictionary<string, object>? Details { get; }
    }
}
