using System;
using System.Reflection;
using CommandQuery.DependencyInjection.Internal;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.DependencyInjection
{
    public class ReflectionExtensionsTests
    {
        [LoFu, Test]
        public void when_GetHandlers()
        {
            Assembly = typeof(FakeCommandHandler).GetTypeInfo().Assembly;

            void should_return_command_handlers_from_Assembly()
            {
                Assembly.GetHandlers(typeof(ICommandHandler<>)).Should().Contain(typeof(FakeCommandHandler));

                Assembly.GetHandlers(typeof(ICommandHandler<,>)).Should().Contain(typeof(FakeResultCommandHandler));
            }

            void should_return_query_handlers_from_Assembly()
            {
                Assembly.GetHandlers(typeof(IQueryHandler<,>)).Should().Contain(typeof(FakeQueryHandler));
            }
        }

        [LoFu, Test]
        public void when_GetHandlerInterface()
        {
            CommandHandler = typeof(ICommandHandler<>);
            CommandHandlerWithResult = typeof(ICommandHandler<,>);
            QueryHandler = typeof(IQueryHandler<,>);

            void should_get_command_handlers()
            {
                typeof(FakeCommandHandler).GetHandlerInterface(CommandHandler).Should().Be(typeof(ICommandHandler<FakeCommand>));

                typeof(FakeResultCommandHandler).GetHandlerInterface(CommandHandlerWithResult).Should().Be(typeof(ICommandHandler<FakeResultCommand, FakeResult>));
            }   
            
            void should_get_query_handlers()
            {
                typeof(FakeQueryHandler).GetHandlerInterface(QueryHandler).Should().Be(typeof(IQueryHandler<FakeQuery, FakeResult>));
            }
        }

        Assembly Assembly;
        Type CommandHandler;
        Type CommandHandlerWithResult;
        Type QueryHandler;
    }
}