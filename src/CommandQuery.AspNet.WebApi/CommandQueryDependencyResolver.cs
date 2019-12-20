using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dependencies;
using CommandQuery.AspNet.WebApi.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.AspNet.WebApi
{
    /// <summary>
    /// Dependency resolver for the <see cref="System.Web.Http.HttpConfiguration" />.
    /// </summary>
    public class CommandQueryDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandQueryDependencyResolver" /> class.
        /// </summary>
        /// <param name="services">An <see cref="IServiceCollection" /></param>
        public CommandQueryDependencyResolver(IServiceCollection services)
        {
            services.AddServiceProvider();
            services.AddControllers(Assembly.GetCallingAssembly());

            _provider = services.BuildServiceProvider();
        }

        private CommandQueryDependencyResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>Retrieves a service from the scope.</summary>
        /// <param name="serviceType">The service to be retrieved.</param>
        /// <returns>The retrieved service.</returns>
        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }

        /// <summary>Retrieves a collection of services from the scope.</summary>
        /// <param name="serviceType">The collection of services to be retrieved.</param>
        /// <returns>The retrieved collection of services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _provider.GetServices(serviceType);
        }

        /// <summary>Starts a resolution scope.</summary>
        /// <returns>The dependency scope.</returns>
        public IDependencyScope BeginScope()
        {
            return new CommandQueryDependencyResolver(_provider.CreateScope().ServiceProvider);
        }

        /// <summary>Releases resources.</summary>
        public void Dispose()
        {
        }
    }
}