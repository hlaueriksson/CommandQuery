using System.Reflection;
using CommandQuery.DependencyInjection;
using LoFuUnit.NUnit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests.DependencyInjection
{
    public class ServiceCollectionExtensionsTests
    {
        [LoFu, Test]
        public void when_AddCommands()
        {
            Assembly = typeof(FakeCommandHandler).GetTypeInfo().Assembly;

            void should_add_commands_from_Assemblies()
            {
                var serviceCollectionMock = new Mock<IServiceCollection>();

                serviceCollectionMock.Object.AddCommands(Assembly);

                serviceCollectionMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(y => y.ServiceType == typeof(ICommandHandler<FakeCommand>) && y.ImplementationType == typeof(FakeCommandHandler))));
                serviceCollectionMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(y => y.ServiceType == typeof(ICommandHandler<FakeResultCommand, FakeResult>) && y.ImplementationType == typeof(FakeResultCommandHandler))));
            }
        }

        [LoFu, Test]
        public void when_AddQueries()
        {
            Assembly = typeof(FakeQueryHandler).GetTypeInfo().Assembly;

            void should_add_queries_from_Assemblies()
            {
                var serviceCollectionMock = new Mock<IServiceCollection>();

                serviceCollectionMock.Object.AddQueries(Assembly);

                serviceCollectionMock.Verify(x => x.Add(It.Is<ServiceDescriptor>(y => y.ServiceType == typeof(IQueryHandler<FakeQuery, FakeResult>) && y.ImplementationType == typeof(FakeQueryHandler))));
            }
        }

        Assembly Assembly;
    }
}