using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;
using CommandQuery.AspNet.WebApi;
using CommandQuery.Sample.Commands;
using CommandQuery.Sample.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Sample.AspNet.WebApi
{
    public static class DependencyInjectionHttpConfigurationExtensions
    {
        public static void UseDependencyInjection(this HttpConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddTransient<IServiceProvider>(_ => services.BuildServiceProvider());

            services.AddCommands(typeof(FooCommand).Assembly);
            services.AddQueries(typeof(BarQuery).Assembly);

            services.AddTransient<IDateTimeProxy, DateTimeProxy>();

            var controllerTypes = Assembly.GetExecutingAssembly().GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Where(t => typeof(ApiController).IsAssignableFrom(t) || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase));
            foreach (var type in controllerTypes)
            {
                services.AddTransient(type);
            }

            configuration.DependencyResolver = new DependencyInjectionDependencyResolver(services.BuildServiceProvider());
        }
    }

    public class DependencyInjectionDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _provider;

        public DependencyInjectionDependencyResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _provider.GetServices(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return new DependencyInjectionDependencyResolver(_provider.CreateScope().ServiceProvider);
        }

        public void Dispose()
        {
        }
    }
}