using System;
using System.Reflection;
using CommandQuery.DependencyInjection;
using LoFuUnit.NUnit;
using Moq;
using NUnit.Framework;

namespace CommandQuery.Tests.DependencyInjection
{
    public class GenericContainerExtensionsTests
    {
        [LoFu, Test]
        public void when_RegisterCommands()
        {
            Assembly = typeof(FakeCommandHandler).GetTypeInfo().Assembly;

            void should_add_commands_from_Assemblies()
            {
                var registerTypeMock = new Mock<Action<Type, Type>>();
                var registerInstanceMock = new Mock<Action<Type, object>>();

                new[] { Assembly }.RegisterCommands(registerTypeMock.Object, registerInstanceMock.Object);

                registerTypeMock.Verify(action => action(typeof(ICommandHandler<FakeCommand>), typeof(FakeCommandHandler)));
                registerTypeMock.Verify(action => action(typeof(ICommandHandler<FakeResultCommand, FakeResult>), typeof(FakeResultCommandHandler)));
            }
        }

        [LoFu, Test]
        public void when_RegisterQueries()
        {
            Assembly = typeof(FakeQueryHandler).GetTypeInfo().Assembly;

            void should_add_queries_from_Assemblies()
            {
                var registerTypeMock = new Mock<Action<Type, Type>>();
                var registerInstanceMock = new Mock<Action<Type, object>>();

                new[] { Assembly }.RegisterQueries(registerTypeMock.Object, registerInstanceMock.Object);

                registerTypeMock.Verify(action => action(typeof(IQueryHandler<FakeQuery, FakeResult>), typeof(FakeQueryHandler)));
            }
        }

        Assembly Assembly;
    }
}