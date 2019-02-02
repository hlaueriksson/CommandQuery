namespace CommandQuery.Sample.Contracts.Queries
{
    public class Bar
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }

    public class BarQuery : IQuery<Bar>
    {
        public int Id { get; set; }
    }
}