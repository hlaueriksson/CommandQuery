using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.AspNet.WebApi
{
    /// <summary>
    /// Extensions methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a <see cref="IServiceProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/></param>
        /// <returns>The <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddServiceProvider(this IServiceCollection services)
        {
            services.AddTransient<IServiceProvider>(_ => services.BuildServiceProvider());

            return services;
        }

        /// <summary>
        /// Adds controllers to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/></param>
        /// <param name="assembly">The <see cref="Assembly"/> with controllers</param>
        /// <returns>The <see cref="IServiceCollection"/></returns>
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