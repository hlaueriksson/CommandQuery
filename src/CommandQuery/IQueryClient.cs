using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CommandQuery
{
    public interface IQueryClient
    {
        string BaseUrl { get; set; }

        Task<TResult> PostAsync<TResult>(IQuery<TResult> query);

        Task<JToken> PostAsync(string slug, object query);
    }

    public class QueryClient : BaseClient, IQueryClient
    {
        public QueryClient(string baseUri) : base(baseUri)
        {
        }

        public async Task<TResult> PostAsync<TResult>(IQuery<TResult> query)
        {
            return await QueryPostAsync<TResult>(query);
        }

        public async Task<JToken> PostAsync(string slug, object query)
        {
            return await QueryPostAsync(slug, query);
        }
    }

    public class QueryClientSettings
    {
        public Dictionary<string, string> BaseUrl { get; set; }
    }
}