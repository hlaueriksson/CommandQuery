using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.AspNet.WebApi
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceProvider(this IServiceCollection services)
        {
            services.AddTransient<IServiceProvider>(_ => services.BuildServiceProvider());

            return services;
        }

        public static IServiceCollection AddControllers(this IServiceCollection services, Assembly assembly = null)
        {
            var controllerTypes = (assembly ?? Assembly.GetCallingAssembly()).GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(ApiController).IsAssignableFrom(t) || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase));
            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }

            return services;
        }
    }
}