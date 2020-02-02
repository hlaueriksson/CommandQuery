using System;
using CommandQuery.Internal;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.Extensions.DependencyInjection;
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

            void should_throw_ArgumentNullException_when_IServiceProvider_is_null() =>
                ((IServiceProvider)null).Invoking(x => x.GetSingleService(typeof(ICommandHandler<FakeMultiCommand1>))).Should().Throw<ArgumentNullException>();

            void should_throw_ArgumentNullException_when_the_service_type_is_null() =>
                ServiceProvider.Invoking(x => x.GetSingleService(null)).Should().Throw<ArgumentNullException>();

            void should_return_the_service_when_only_one_service_is_found() =>
                ServiceProvider.GetSingleService(typeof(ICommandHandler<FakeMultiCommand1>)).Should().BeOfType<FakeMultiHandler>();

            void should_return_null_when_no_service_is_found() =>
                ServiceProvider.GetSingleService(typeof(ICommandHandler<FakeMultiCommand2>)).Should().BeNull();

            void should_throw_InvalidOperationException_when_multiple_services_are_found() =>
                ServiceProvider.Invoking(x => x.GetSingleService(typeof(IQueryHandler<FakeMultiQuery1, FakeResult>))).Should().Throw<InvalidOperationException>();
        }

        IServiceProvider ServiceProvider;
    }
}