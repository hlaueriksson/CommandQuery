using System;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.Internal
{
    public class ExceptionExtensionsTests
    {
        [LoFu, Test]
        public void IsHandled()
        {
            void should_return_true_for_custom_exceptions()
            {
                new CommandException("").IsHandled().Should().BeTrue();
                new CommandProcessorException("").IsHandled().Should().BeTrue();
                new QueryException("").IsHandled().Should().BeTrue();
                new QueryProcessorException("").IsHandled().Should().BeTrue();
            }

            void should_return_false_for_other_exceptions() => new Exception().IsHandled().Should().BeFalse();
        }

        [LoFu, Test]
        public void GetCommandEventId()
        {
            void should_return_distinct_ids_based_on_exception_type()
            {
                new[]
                {
                    new CommandException("").GetCommandEventId(),
                    new CommandProcessorException("").GetCommandEventId(),
                    new Exception().GetCommandEventId()
                }.Should().OnlyHaveUniqueItems();
            }
        }

        [LoFu, Test]
        public void GetQueryEventId()
        {
            void should_return_distinct_ids_based_on_exception_type()
            {
                new[]
                {
                    new QueryException("").GetQueryEventId(),
                    new QueryProcessorException("").GetQueryEventId(),
                    new Exception().GetQueryEventId()
                }.Should().OnlyHaveUniqueItems();
            }
        }
        
        [LoFu, Test]
        public void GetCommandCategory()
        {
            void should_return_distinct_categories_based_on_exception_type()
            {
                new[]
                {
                    new CommandException("").GetCommandCategory(),
                    new CommandProcessorException("").GetCommandCategory(),
                    new Exception().GetCommandCategory()
                }.Should().OnlyHaveUniqueItems();
            }
        }

        [LoFu, Test]
        public void GetQueryCategory()
        {
            void should_return_distinct_categories_based_on_exception_type()
            {
                new[]
                {
                    new QueryException("").GetQueryCategory(),
                    new QueryProcessorException("").GetQueryCategory(),
                    new Exception().GetQueryCategory()
                }.Should().OnlyHaveUniqueItems();
            }
        }
    }
}