namespace CommandQuery.Sample.AspNet.WebApi.Contracts.Commands
{
    public class FooCommand : ICommand
    {
        public string Value { get; set; }
    }
}