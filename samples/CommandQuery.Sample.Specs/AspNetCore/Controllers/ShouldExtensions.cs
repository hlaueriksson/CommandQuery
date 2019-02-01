#if NETCOREAPP2_0
using System.Net.Http;
using Machine.Specifications;

namespace CommandQuery.Sample.Specs.AspNetCore.Controllers
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this HttpResponseMessage result, string message)
        {
            result.ShouldNotBeNull();
            result.IsSuccessStatusCode.ShouldBeFalse();
            var value = result.Content.ReadAsAsync<Error>().Result;
            value.ShouldNotBeNull();
            value.Message.ShouldEqual(message);
        }
    }
}
#endif