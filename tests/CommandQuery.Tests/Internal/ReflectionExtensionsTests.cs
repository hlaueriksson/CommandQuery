using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Assembly = typeof(FakeCommandHandler).Assembly;

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
        public void when_GetHandlerInterfaces()
        {
            typeof(FakeCommandHandler).GetHandlerInterfaces(typeof(ICommandHandler<>)).Should().BeEquivalentTo(typeof(ICommandHandler<FakeCommand>));
            typeof(FakeResultCommandHandler).GetHandlerInterfaces(typeof(ICommandHandler<,>)).Should().BeEquivalentTo(typeof(ICommandHandler<FakeResultCommand, FakeResult>));
            typeof(FakeQueryHandler).GetHandlerInterfaces(typeof(IQueryHandler<,>)).Should().BeEquivalentTo(typeof(IQueryHandler<FakeQuery, FakeResult>));

            typeof(FakeMultiHandler).GetHandlerInterfaces(typeof(ICommandHandler<>)).Count().Should().Be(2);
            typeof(FakeMultiHandler).GetHandlerInterfaces(typeof(ICommandHandler<,>)).Count().Should().Be(2);
            typeof(FakeMultiHandler).GetHandlerInterfaces(typeof(IQueryHandler<,>)).Count().Should().Be(2);
        }

        Assembly Assembly;
    }
}
