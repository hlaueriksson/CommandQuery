using System.Collections.Generic;
using System.Reflection;
using CommandQuery.Internal;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.Internal
{
    public class ReflectionExtensionsTests
    {
        [LoFu, Test]
        public void when_GetTypesAssignableTo()
        {
            Assembly = typeof(FakeCommandHandler).GetTypeInfo().Assembly;

            void should_return_handlers_from_Assemblies()
            {
                new[] { Assembly }.GetTypesAssignableTo(typeof(ICommandHandler<>)).Should().Contain(typeof(FakeCommandHandler));
                new[] { Assembly }.GetTypesAssignableTo(typeof(ICommandHandler<,>)).Should().Contain(typeof(FakeResultCommandHandler));
                new[] { Assembly }.GetTypesAssignableTo(typeof(IQueryHandler<,>)).Should().Contain(typeof(FakeQueryHandler));
            }

            void should_return_handlers_from_Assembly()
            {
                Assembly.GetTypesAssignableTo(typeof(ICommandHandler<>)).Should().Contain(typeof(FakeCommandHandler));
                Assembly.GetTypesAssignableTo(typeof(ICommandHandler<,>)).Should().Contain(typeof(FakeResultCommandHandler));
                Assembly.GetTypesAssignableTo(typeof(IQueryHandler<,>)).Should().Contain(typeof(FakeQueryHandler));
            }
        }

        [Test]
        public void when_IsAssignableTo()
        {
            typeof(FakeCommand).IsAssignableTo(typeof(ICommand)).Should().BeTrue();
            typeof(FakeResultCommand).IsAssignableTo(typeof(ICommand<>)).Should().BeTrue();
            typeof(FakeQuery).IsAssignableTo(typeof(IQuery<>)).Should().BeTrue();

            typeof(FakeResult).IsAssignableTo(typeof(IEnumerable<>)).Should().BeFalse();
        }

        [Test]
        public void when_GetReturnType()
        {
            typeof(FakeResultCommand).GetResultType(typeof(ICommand<>)).Should().Be(typeof(FakeResult));
            typeof(FakeCommand).GetResultType(typeof(ICommand)).Should().BeNull();
        }

        [Test]
        public void when_GetHandlerInterface()
        {
            typeof(FakeCommandHandler).GetHandlerInterface(typeof(ICommandHandler<>)).Should().Be(typeof(ICommandHandler<FakeCommand>));
            typeof(FakeResultCommandHandler).GetHandlerInterface(typeof(ICommandHandler<,>)).Should().Be(typeof(ICommandHandler<FakeResultCommand, FakeResult>));
            typeof(FakeQueryHandler).GetHandlerInterface(typeof(IQueryHandler<,>)).Should().Be(typeof(IQueryHandler<FakeQuery, FakeResult>));
        }

        Assembly Assembly;
    }
}