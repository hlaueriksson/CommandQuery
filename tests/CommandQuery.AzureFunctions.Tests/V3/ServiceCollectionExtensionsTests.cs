#if NETCOREAPP3_1
using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.AzureFunctions.Tests.V3
{
    public class ServiceCollectionExtensionsTests
    {
        [Test]
        public void when_AddCommandFunction()
        {
            var assembly = typeof(FakeCommandHandler).Assembly;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddCommandFunction(assembly);
            var provider = serviceCollection.BuildServiceProvider();

            provider.GetService<ICommandFunction>().Should().NotBeNull();
        }

        [Test]
        public void when_AddQueryFunction()
        {
            var assembly = typeof(FakeQueryHandler).Assembly;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddQueryFunction(assembly);
            var provider = serviceCollection.BuildServiceProvider();

            provider.GetService<IQueryFunction>().Should().NotBeNull();
        }
    }
}
#endif
