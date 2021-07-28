using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;

namespace CommandQuery.Sample.AzureFunctions.V5.Tests
{
    public static class TestExtensions
    {
        public static async Task<T> AsAsync<T>(this HttpResponseData result)
        {
            result.Body.Position = 0;
            return await JsonSerializer.DeserializeAsync<T>(result.Body);
        }
    }
}
