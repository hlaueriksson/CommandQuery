namespace CommandQuery.Sample.AspNet.WebApi.Contracts.Queries
{
    public class BarQuery : IQuery<Bar>
    {
        public int Id { get; set; }
    }

    public class Bar
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }
}