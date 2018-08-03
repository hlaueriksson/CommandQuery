#if NET461
using System.Net;
using System.Threading;
using System.Web.Http;
using Machine.Specifications;

namespace CommandQuery.Specs.AspNet.WebApi
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this IHttpActionResult result, string message)
        {
            var response = result.ExecuteAsync(CancellationToken.None).Result;
            response.ShouldNotBeNull();
            response.StatusCode.ShouldNotEqual(HttpStatusCode.OK);
            var value = response.Content.ReadAsStringAsync().Result;
            value.ShouldNotBeNull();
            value.ShouldEqual(message);
        }
    }
}
#endif