namespace CommandQuery.Sample.Contracts
{
    public class Error : IError
    {
        public string Message { get; set; }
        public Dictionary<string, object> Details { get; set; }
    }
}
