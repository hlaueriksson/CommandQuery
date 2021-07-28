using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CommandQuery.Sample.GoogleCloudFunctions.Tests
{
    public static class TestExtensions
    {
        public static async Task<T> AsAsync<T>(this HttpResponse result)
        {
            result.Body.Position = 0;
            return await JsonSerializer.DeserializeAsync<T>(result.Body);
        }
    }
}
