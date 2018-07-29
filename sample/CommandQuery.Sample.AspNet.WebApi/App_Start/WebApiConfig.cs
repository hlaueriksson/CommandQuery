using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using CommandQuery.AspNet.WebApi;
using CommandQuery.Sample.Commands;
using CommandQuery.Sample.Queries;
using Unity;

namespace CommandQuery.Sample.AspNet.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // IoC
            var container = new UnityContainer();

            container.RegisterType<IServiceProvider, UnityServiceProvider>();

            container.RegisterCommands(typeof(FooCommand).Assembly);
            container.RegisterQueries(typeof(BarQuery).Assembly);

            container.RegisterType<IDateTimeProxy, DateTimeProxy>();

            config.DependencyResolver = new UnityDependencyResolver(container);

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