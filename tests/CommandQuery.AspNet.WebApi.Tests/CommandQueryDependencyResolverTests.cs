using System;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.AspNet.WebApi.Tests
{
    public class CommandQueryDependencyResolverTests
    {
        [SetUp]
        public void SetUp() => Subject = new CommandQueryDependencyResolver(new ServiceCollection());

        [LoFu, Test]
        public void when_GetService()
        {
            void should_return_the_instance() => Subject.GetService(typeof(IServiceProvider)).Should().NotBeNull();
        }

        [LoFu, Test]
        public void when_GetServices()
        {
            void should_return_all_instances() => Subject.GetServices(typeof(IServiceProvider)).Should().NotBeEmpty();
        }

        [LoFu, Test]
        public void when_BeginScope()
        {
            void should_return_a_clone_of_itself() => Subject.BeginScope().Should().NotBeNull();
        }

        [LoFu, Test]
        public void when_Dispose()
        {
            void should_do_nothing() => Subject.Dispose();
        }

        CommandQueryDependencyResolver Subject;
    }
}