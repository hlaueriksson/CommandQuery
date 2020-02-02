using System;
using System.Linq;
using CommandQuery.AspNet.WebApi.Internal;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.AspNet.WebApi.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [LoFu, Test]
        public void when_AddServiceProvider()
        {
            Subject = new ServiceCollection();
            Subject.AddServiceProvider();

            void should_build_a_ServiceProvider_and_add_it_to_itself()
            {
                Subject.Any(x => x.ServiceType == typeof(IServiceProvider)).Should().BeTrue();
            }
        }

        [LoFu, Test]
        public void when_AddControllers()
        {
            Subject = new ServiceCollection();
            Subject.AddControllers(typeof(FakeCommandController).Assembly);

            void should_add_controllers_from_the_given_assembly()
            {
                Subject.Any(x => x.ServiceType == typeof(FakeCommandController)).Should().BeTrue();
                Subject.Any(x => x.ServiceType == typeof(FakeQueryController)).Should().BeTrue();
            }
        }

        ServiceCollection Subject;
    }
}