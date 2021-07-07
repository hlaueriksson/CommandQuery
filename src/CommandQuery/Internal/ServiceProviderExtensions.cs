using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
