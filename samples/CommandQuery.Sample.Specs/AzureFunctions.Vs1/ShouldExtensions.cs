#if NET461
using System.Net.Http;
using CommandQuery.AzureFunctions;
using Machine.Specifications;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs1
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