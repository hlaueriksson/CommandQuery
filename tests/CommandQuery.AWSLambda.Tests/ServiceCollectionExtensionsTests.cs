using CommandQuery.Tests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.AWSLambda.Tests
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
