using System;

namespace CommandQuery.Sample.Contracts.Queries
{
    public class QuuxQuery : IQuery<Quux>
    {
        public long? Id { get; set; }
        public Corge Corge { get; set; }
    }

    public class Corge
    {
        public DateTime DateTime { get; set; }
        public Grault Grault { get; set; }
    }

    public class Grault
    {
        public DayOfWeek DayOfWeek { get; set; }
    }

    public class Quux
    {
        public long Id { get; set; }
        public Corge Corge { get; set; }
    }
}
