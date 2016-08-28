using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommandQuery
{
    public abstract class BaseClient
    {
        public string BaseUrl { get; set; }

        protected BaseClient(string baseUri)
        {
            BaseUrl = baseUri;
        }

        protected async Task CommandPostAsync(object value)
        {
            await BasePostAsync(value);
        }

        protected async Task<TResult> QueryPostAsync<TResult>(object value)
        {
            var response = await BasePostAsync(value);

            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(responseString);
        }

        private async Task<HttpResponseMessage> BasePostAsync(object value)
        {
            var uri = BaseUrl + value.GetType().Name;
            var content = JsonConvert.SerializeObject(value);

            var client = new HttpClient();
            var response = await client.PostAsync(uri, new StringContent(content, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return response;
        }

        protected async Task CommandPostAsync(string slug, object value)
        {
            await BasePostAsync(slug, value);
        }

        protected async Task<JToken> QueryPostAsync(string slug, object value)
        {
            var response = await BasePostAsync(slug, value);

            var responseString = await response.Content.ReadAsStringAsync();

            return JToken.Parse(responseString);
        }

        private async Task<HttpResponseMessage> BasePostAsync(string slug, object value)
        {
            var uri = BaseUrl + slug;
            var content = JsonConvert.SerializeObject(value);

            var client = new HttpClient();
            var response = await client.PostAsync(uri, new StringContent(content, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}