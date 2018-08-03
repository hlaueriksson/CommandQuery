#if NETCOREAPP2_0
using CommandQuery.AspNetCore;
using Machine.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Specs.AspNetCore
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