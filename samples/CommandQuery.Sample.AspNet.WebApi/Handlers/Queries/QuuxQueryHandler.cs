using System.Threading.Tasks;
using CommandQuery.Sample.AspNet.WebApi.Contracts.Queries;

namespace CommandQuery.Sample.AspNet.WebApi.Handlers.Queries
{
    public class QuuxQueryHandler : IQueryHandler<QuuxQuery, Quux>
    {
        public async Task<Quux> HandleAsync(QuuxQuery query)
        {
            if (query.Corge is null) throw new QuuxQueryException("Corge is null") { InvalidCorge = true };
            if (query.Corge.Grault is null) throw new QuuxQueryException("Grault is null") { InvalidGrault = true };

            var result = new Quux
            {
                Id = query.Id.GetValueOrDefault(1337),
                Corge = new Corge
                {
                    DateTime = query.Corge.DateTime.AddDays(1),
                    Grault = new Grault
                    {
                        DayOfWeek = query.Corge.Grault.DayOfWeek + 1
                    }
                }
            };

            return await Task.FromResult(result);
        }
    }
}
