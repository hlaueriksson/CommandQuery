using System;
using System.Linq;
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
            void should_add_controllers_from_the_calling_assembly()
            {
                Subject = new ServiceCollection();
                Subject.AddControllers();
                Subject.Any(x => x.ServiceType == typeof(FakeCommandController)).Should().BeTrue();
                Subject.Any(x => x.ServiceType == typeof(FakeQueryController)).Should().BeTrue();
            }

            void should_add_controllers_from_the_given_assembly()
            {
                Subject = new ServiceCollection();
                Subject.AddControllers(typeof(FakeCommandController).Assembly);
                Subject.Any(x => x.ServiceType == typeof(FakeCommandController)).Should().BeTrue();
                Subject.Any(x => x.ServiceType == typeof(FakeQueryController)).Should().BeTrue();
            }
        }

        ServiceCollection Subject;
    }
}