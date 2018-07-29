using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
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

            // Json
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            // Web API routes
            config.MapHttpAttributeRoutes(new CustomDirectRouteProvider());
        }
    }

    public class CustomDirectRouteProvider : DefaultDirectRouteProvider
    {
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor.GetCustomAttributes<IDirectRouteFactory>(inherit: true);
        }
    }
}