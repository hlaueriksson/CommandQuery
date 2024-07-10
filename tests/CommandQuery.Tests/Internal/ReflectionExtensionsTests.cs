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
        public void when_IsAssignableToType()
        {
            typeof(FakeCommand).IsAssignableToType(typeof(ICommand)).Should().BeTrue();
            typeof(FakeResultCommand).IsAssignableToType(typeof(ICommand<>)).Should().BeTrue();
            typeof(FakeQuery).IsAssignableToType(typeof(IQuery<>)).Should().BeTrue();

            typeof(FakeResult).IsAssignableToType(typeof(IEnumerable<>)).Should().BeFalse();
        }

        [Test]
        public void when_GetReturnType()
        {
            typeof(FakeResultCommand).GetResultType(typeof(ICommand<>)).Should().Be(typeof(FakeResult));
            typeof(FakeCommand).GetResultType(typeof(ICommand)).Should().BeNull();
        }

        [Test]
        public void when_GetHandlerInterfaceTypes()
        {
            typeof(FakeCommandHandler).GetHandlerInterfaceTypes(typeof(ICommandHandler<>)).Should().AllBeEquivalentTo(typeof(ICommandHandler<FakeCommand>));
            typeof(FakeResultCommandHandler).GetHandlerInterfaceTypes(typeof(ICommandHandler<,>)).Should().AllBeEquivalentTo(typeof(ICommandHandler<FakeResultCommand, FakeResult>));
            typeof(FakeQueryHandler).GetHandlerInterfaceTypes(typeof(IQueryHandler<,>)).Should().AllBeEquivalentTo(typeof(IQueryHandler<FakeQuery, FakeResult>));

            typeof(FakeMultiHandler).GetHandlerInterfaceTypes(typeof(ICommandHandler<>)).Count().Should().Be(2);
            typeof(FakeMultiHandler).GetHandlerInterfaceTypes(typeof(ICommandHandler<,>)).Count().Should().Be(2);
            typeof(FakeMultiHandler).GetHandlerInterfaceTypes(typeof(IQueryHandler<,>)).Count().Should().Be(2);
        }

        Assembly Assembly;
    }
}
