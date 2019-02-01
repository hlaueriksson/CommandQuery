#if NET461
using System.Net;
using System.Net.Http;
using CommandQuery.AzureFunctions;
using Machine.Specifications;

namespace CommandQuery.Specs.AzureFunctions.V1
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this HttpResponseMessage result, string message)
        {
            result.ShouldNotBeNull();
            result.StatusCode.ShouldNotEqual(HttpStatusCode.OK);
            var value = result.Content.ReadAsAsync<Error>().Result;
            value.ShouldNotBeNull();
            value.Message.ShouldEqual(message);
        }
    }
}
#endif