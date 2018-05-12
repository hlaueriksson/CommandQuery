#if NETCOREAPP2_0
using Machine.Specifications;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs2
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this ObjectResult result, string message)
        {
            result.ShouldNotBeNull();
            result.StatusCode.ShouldNotEqual(200);
            var value = JsonConvert.SerializeObject(result.Value);
            value.ShouldNotBeNull();
            value.ShouldContain(message);
        }
    }
}
#endif