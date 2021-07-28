using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CommandQuery.Sample.AzureFunctions.V3.Tests
{
    public static class TestExtensions
    {
        public static T As<T>(this ContentResult result)
        {
            return JsonConvert.DeserializeObject<T>(result.Content);
        }
    }
}
