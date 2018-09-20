using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.AspNet.WebApi
{
    public class CommandQueryDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _provider;

        public CommandQueryDependencyResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public CommandQueryDependencyResolver(IServiceCollection services)
        {
            services.AddServiceProvider();
            services.AddControllers(Assembly.GetCallingAssembly());

            _provider = services.BuildServiceProvider();
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
            return new CommandQueryDependencyResolver(_provider.CreateScope().ServiceProvider);
        }

        public void Dispose()
        {
        }
    }
}