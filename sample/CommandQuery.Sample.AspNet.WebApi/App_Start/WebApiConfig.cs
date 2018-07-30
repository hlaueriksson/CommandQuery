using System.Net.Http.Formatting;
using System.Web.Http;
using CommandQuery.AspNet.WebApi;

namespace CommandQuery.Sample.AspNet.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // IoC
            config.UseUnity();

            // Log
            config.EnableSystemDiagnosticsTracing();

            // Json
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            // Web API routes
            config.MapHttpAttributeRoutes(new CommandQueryDirectRouteProvider());
        }
    }
}