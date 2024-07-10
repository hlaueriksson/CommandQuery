using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests.Internal
{
    public class ServiceProviderExtensionsTests
    {
        [LoFu, Test]
        public void when_GetSingleService()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ICommandHandler<FakeMultiCommand1>, FakeMultiHandler>();
            serviceCollection.AddTransient<IQueryHandler<FakeMultiQuery1, FakeResult>, FakeMultiHandler>();
            serviceCollection.AddTransient<IQueryHandler<FakeMultiQuery1, FakeResult>, FakeMultiHandler>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            void should_throw_ArgumentNullException_when_IServiceProvider_is_null()
            {
                Action act = () => ((IServiceProvider)null).GetSingleService(typeof(ICommandHandler<FakeMultiCommand1>));
                act.Should().Throw<ArgumentNullException>();
            }

            void should_throw_ArgumentNullException_when_the_service_type_is_null() =>
                ServiceProvider.Invoking(x => x.GetSingleService(null)).Should().Throw<ArgumentNullException>();

            void should_return_the_service_when_only_one_service_is_found() =>
                ServiceProvider.GetSingleService(typeof(ICommandHandler<FakeMultiCommand1>)).Should().BeOfType<FakeMultiHandler>();

            void should_return_null_when_no_service_is_found()
            {
                ServiceProvider.GetSingleService(typeof(ICommandHandler<FakeMultiCommand2>)).Should().BeNull();
                ServiceProvider.GetSingleService(typeof(int)).Should().BeNull();
            }

            void should_throw_InvalidOperationException_when_multiple_services_are_found() =>
                ServiceProvider.Invoking(x => x.GetSingleService(typeof(IQueryHandler<FakeMultiQuery1, FakeResult>))).Should().Throw<InvalidOperationException>();

            void should_throw_NotSupportedException_when_service_type_is_open() =>
                ServiceProvider.Invoking(x => x.GetSingleService(typeof(IEnumerable<>))).Should().Throw<NotSupportedException>();
        }

        [LoFu, Test]
        public void when_GetAllServiceTypes()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ICommandHandler<FakeMultiCommand1>, FakeMultiHandler>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            void should_return_empty_enumeration_when_IServiceProvider_is_null() =>
                ((IServiceProvider)null).GetAllServiceTypes().Should().BeEmpty();

            void should_return_empty_enumeration_if_IServiceProvider_does_not_have_the_right_private_members()
            {
                var broken = new Mock<IServiceProvider>().Object;
                broken.GetAllServiceTypes().Should().BeEmpty();
            }

            void should_return_all_service_types() =>
                ServiceProvider.GetAllServiceTypes().Should().Contain(typeof(ICommandHandler<FakeMultiCommand1>));
        }

        IServiceProvider ServiceProvider;
    }
}
