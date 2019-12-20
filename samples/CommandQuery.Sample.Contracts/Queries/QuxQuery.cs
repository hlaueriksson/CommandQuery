using System;

namespace CommandQuery.Sample.Contracts.Queries
{
    public class QuxQuery : IQuery<Qux[]>
    {
        public Guid[] Ids { get; set; }
    }

    public class Qux
    {
        public Guid Id { get; set; }

        public string Value { get; set; }
    }
}