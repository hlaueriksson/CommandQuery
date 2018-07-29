using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dependencies;
using CommandQuery.AspNet.WebApi;
using Unity;
using Unity.Exceptions;

namespace CommandQuery.Sample.AspNet.WebApi
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;

        public UnityDependencyResolver(IUnityContainer container)
        {
            _container = container ?? throw new ArgumentNullException("container");
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = _container.CreateChildContainer();
            return new UnityDependencyResolver(child);
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }

    public class UnityServiceProvider : IServiceProvider
    {
        private readonly IUnityContainer _container;

        public UnityServiceProvider(IUnityContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }
    }

    public static class UnityContainerExtensions
    {
        public static IUnityContainer RegisterCommands(this IUnityContainer container, params Assembly[] assemblies)
        {
            assemblies.RegisterCommands((from, to) => container.RegisterType(from, to), (from, to) => container.RegisterInstance(from, to));

            return container;
        }

        public static IUnityContainer RegisterQueries(this IUnityContainer container, params Assembly[] assemblies)
        {
            assemblies.RegisterQueries((from, to) => container.RegisterType(from, to), (from, to) => container.RegisterInstance(from, to));

            return container;
        }
    }
}