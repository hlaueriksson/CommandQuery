using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandQuery.Internal
{
    internal static class ServiceProviderExtensions
    {
        public static object GetSingleService(this IServiceProvider provider, Type serviceType)
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
            var services = (IEnumerable<object>)provider.GetService(enumerableType);

            return services?.SingleOrDefault();
        }
    }
}
