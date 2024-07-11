using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery
{
    internal static class ServiceProviderExtensions
    {
        internal static object? GetSingleService(this IServiceProvider provider, Type serviceType)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            var services = provider.GetService(enumerableType) as IEnumerable<object>;

            return services?.SingleOrDefault();
        }

        internal static IEnumerable<Type> GetAllServiceTypes(this IServiceProvider provider)
        {
            if (provider is not ServiceProvider serviceProvider)
            {
                return [];
            }

            var callSiteFactory = GetPropertyValue(serviceProvider, "CallSiteFactory");
            var descriptors = GetPropertyValue(callSiteFactory, "Descriptors");

            if (descriptors is not ServiceDescriptor[] array)
            {
                return [];
            }

            return array.Select(x => x.ServiceType).ToList();

            static object? GetPropertyValue(object? obj, string name)
            {
                return obj?.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj, null);
            }
        }
    }
}
