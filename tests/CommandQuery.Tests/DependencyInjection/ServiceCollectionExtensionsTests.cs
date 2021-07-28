using System.Reflection;
using CommandQuery.DependencyInjection;
using FluentAssertions;
using LoFuUnit.NUnit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CommandQuery.Tests.DependencyInjection
{
    public class ServiceCollectionExtensionsTests
    {
        [LoFu, Test]
        public void when_AddCommands()
        {
            Assembly = typeof(FakeCommandHandler).Assembly;

            void should_add_commands_from_Assemblies()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCommands(Assembly);
                var provider = serviceCollection.BuildServiceProvider();

                provider.GetService<ICommandHandler<FakeCommand>>().Should().BeOfType<FakeCommandHandler>();
                provider.GetService<ICommandHandler<FakeResultCommand, FakeResult>>().Should().BeOfType<FakeResultCommandHandler>();
            }

            void should_create_a_CommandTypeProvider()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCommands(Assembly);
                var provider = serviceCollection.BuildServiceProvider();

                provider.GetService(typeof(ICommandTypeProvider)).Should().NotBeNull();
            }

            void should_add_all_commands_from_handler()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddCommands(Assembly);
                var provider = serviceCollection.BuildServiceProvider();

                provider.GetService<ICommandHandler<FakeMultiCommand1>>().Should().BeOfType<FakeMultiHandler>();
                provider.GetService<ICommandHandler<FakeMultiCommand2>>().Should().BeOfType<FakeMultiHandler>();
                provider.GetService<ICommandHandler<FakeMultiResultCommand1, FakeResult>>().Should().BeOfType<FakeMultiHandler>();
                provider.GetService<ICommandHandler<FakeMultiResultCommand2, FakeResult>>().Should().BeOfType<FakeMultiHandler>();
            }
        }

        [LoFu, Test]
        public void when_AddQueries()
        {
            Assembly = typeof(FakeQueryHandler).Assembly;

            void should_add_queries_from_Assemblies()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddQueries(Assembly);
                var provider = serviceCollection.BuildServiceProvider();

                provider.GetService<IQueryHandler<FakeQuery, FakeResult>>().Should().BeOfType<FakeQueryHandler>();
            }

            void should_create_a_QueryTypeProvider()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddQueries(Assembly);
                var provider = serviceCollection.BuildServiceProvider();

                provider.GetService(typeof(IQueryTypeProvider)).Should().NotBeNull();
            }

            void should_add_all_queries_from_handler()
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddQueries(Assembly);
                var provider = serviceCollection.BuildServiceProvider();

                provider.GetService<IQueryHandler<FakeMultiQuery1, FakeResult>>().Should().BeOfType<FakeMultiHandler>();
                provider.GetService<IQueryHandler<FakeMultiQuery2, FakeResult>>().Should().BeOfType<FakeMultiHandler>();
            }
        }

        Assembly Assembly;
    }
}
