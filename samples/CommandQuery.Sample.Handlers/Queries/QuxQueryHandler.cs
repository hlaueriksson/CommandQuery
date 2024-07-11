using CommandQuery.Sample.Contracts.Queries;

namespace CommandQuery.Sample.Handlers.Queries
{
    public class QuxQueryHandler : IQueryHandler<QuxQuery, Qux[]>
    {
        private readonly IDateTimeProxy _dateTime;

        public QuxQueryHandler(IDateTimeProxy dateTime)
        {
            _dateTime = dateTime;
        }

        public async Task<Qux[]> HandleAsync(QuxQuery query, CancellationToken cancellationToken)
        {
            var result = query.Ids.Select((x, index) => new Qux { Id = x, Value = _dateTime.Now.AddDays(index).ToString("F") }).ToArray();

            return await Task.FromResult(result);
        }
    }
}
