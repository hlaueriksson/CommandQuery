using CommandQuery.Sample.Contracts.Queries;

namespace CommandQuery.Sample.Handlers.Queries
{
    public class BarQueryHandler : IQueryHandler<BarQuery, Bar>
    {
        private readonly IDateTimeProxy _dateTime;

        public BarQueryHandler(IDateTimeProxy dateTime)
        {
            _dateTime = dateTime;
        }

        public async Task<Bar> HandleAsync(BarQuery query, CancellationToken cancellationToken)
        {
            var result = new Bar { Id = query.Id, Value = _dateTime.Now.ToString("F") };

            return await Task.FromResult(result);
        }
    }
}
