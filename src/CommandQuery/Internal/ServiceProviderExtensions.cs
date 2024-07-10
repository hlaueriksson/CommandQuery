using System.Collections;
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
                return Enumerable.Empty<Type>();
            }

            var callSiteFactory = GetPropertyValue(serviceProvider, "CallSiteFactory");
            var descriptorLookup = GetFieldValue(callSiteFactory, "_descriptorLookup");

            if (descriptorLookup is not IDictionary dictionary)
            {
                return Enumerable.Empty<Type>();
            }

            return dictionary.Keys.Cast<Type>();

            static object? GetFieldValue(object? obj, string name)
            {
                return obj?.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
            }

            static object? GetPropertyValue(object? obj, string name)
            {
                return obj?.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj, null);
            }
        }
    }
}
