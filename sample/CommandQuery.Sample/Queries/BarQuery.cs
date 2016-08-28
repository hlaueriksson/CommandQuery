using System;
using System.Threading.Tasks;

namespace CommandQuery.Sample.Queries
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

    public class BarQueryHandler : IQueryHandler<BarQuery, Bar>
    {
        public async Task<Bar> HandleAsync(BarQuery query)
        {
            var result = new Bar { Id = query.Id, Value = DateTime.Now.ToString("F") }; // TODO: do some real query stuff

            return await Task.FromResult(result);
        }
    }
}