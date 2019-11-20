using System;
using System.Collections.Generic;

namespace CommandQuery.Sample.Contracts.Queries
{
    public class QuxQuery : IQuery<IEnumerable<Qux>>
    {
        public IEnumerable<Guid> Ids { get; set; }
    }

    public class Qux
    {
        public Guid Id { get; set; }

        public string Value { get; set; }
    }
}