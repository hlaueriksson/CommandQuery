using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CommandQuery.Client
{
    public abstract class BaseClient
    {
        protected static readonly HttpClient Client = new HttpClient();

        protected BaseClient(string baseUrl, int timeoutInSeconds = 10)
        {
            if (Client.BaseAddress != null) return;

            Client.BaseAddress = new Uri(baseUrl);
            Client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected T BaseGet<T>(object value)
            => Client.GetAsync("api/" + value.GetRequestUri())
                .ConfigureAwait(false).GetAwaiter().GetResult()
                .EnsureSuccessStatusCode()
                .Content.ReadAsAsync<T>()
                .ConfigureAwait(false).GetAwaiter().GetResult();

        protected async Task<T> BaseGetAsync<T>(object value)
        {
            var response = await Client.GetAsync("api/" + value.GetRequestUri());
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        protected void BasePost(object value)
            => Client.PostAsJsonAsync("api/" + value.GetType().Name, value)
                .ConfigureAwait(false).GetAwaiter().GetResult()
                .EnsureSuccessStatusCode();

        protected async Task BasePostAsync(object value)
        {
            var response = await Client.PostAsJsonAsync("api/" + value.GetType().Name, value);
            response.EnsureSuccessStatusCode();
        }

        protected T BasePost<T>(object value)
            => Client.PostAsJsonAsync("api/" + value.GetType().Name, value)
                .ConfigureAwait(false).GetAwaiter().GetResult()
                .EnsureSuccessStatusCode()
                .Content.ReadAsAsync<T>()
                .ConfigureAwait(false).GetAwaiter().GetResult();

        protected async Task<T> BasePostAsync<T>(object value)
        {
            var response = await Client.PostAsJsonAsync("api/" + value.GetType().Name, value);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }
    }
}