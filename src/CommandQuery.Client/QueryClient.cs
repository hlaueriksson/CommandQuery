using System.Threading.Tasks;

namespace CommandQuery.Client
{
    public interface IQueryClient
    {
        TResult Get<TResult>(IQuery<TResult> query);
        Task<TResult> GetAsync<TResult>(IQuery<TResult> query);
        TResult Post<TResult>(IQuery<TResult> query);
        Task<TResult> PostAsync<TResult>(IQuery<TResult> query);
    }

    public class QueryClient : BaseClient, IQueryClient
    {
        public QueryClient(string baseUrl, int timeoutInSeconds = 10) : base(baseUrl, timeoutInSeconds) { }

        public TResult Get<TResult>(IQuery<TResult> query) => BaseGet<TResult>(query);
        public async Task<TResult> GetAsync<TResult>(IQuery<TResult> query) => await BaseGetAsync<TResult>(query);
        public TResult Post<TResult>(IQuery<TResult> query) => BasePost<TResult>(query);
        public async Task<TResult> PostAsync<TResult>(IQuery<TResult> query) => await BasePostAsync<TResult>(query);
    }
}