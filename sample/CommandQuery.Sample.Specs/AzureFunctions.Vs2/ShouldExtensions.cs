#if NETCOREAPP2_0
using CommandQuery.AzureFunctions;
using Machine.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Sample.Specs.AzureFunctions.Vs2
{
    public static class ShouldExtensions
    {
        public static void ShouldBeError(this ObjectResult result, string message)
        {
            result.ShouldNotBeNull();
            result.StatusCode.ShouldNotEqual(200);
            var value = result.Value as Error;
            value.ShouldNotBeNull();
            value.Message.ShouldEqual(message);
        }
    }
}
#endif