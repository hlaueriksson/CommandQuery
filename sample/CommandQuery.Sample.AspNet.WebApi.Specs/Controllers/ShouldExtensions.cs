using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Machine.Specifications;

namespace CommandQuery.Sample.AspNet.WebApi.Specs.Controllers
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this IHttpActionResult result, string message)
        {
            result.ExecuteAsync(CancellationToken.None).Result.ShouldBeError(message);
        }

        public static void ShouldBeError(this HttpResponseMessage result, string message)
        {
            result.ShouldNotBeNull();
            result.IsSuccessStatusCode.ShouldBeFalse();
            var value = result.Content.ReadAsStringAsync().Result;
            value.ShouldNotBeNull();
            value.ShouldContain(message);
        }
    }
}