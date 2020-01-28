using System;
using System.Collections.Generic;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using FluentAssertions;
using LoFuUnit.NUnit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

        [Test]
        public void when_converting_Exception_to_Error()
        {
            var exception = new Exception("message");

            var result = exception.ToError();

            result.Message.Should().Be(exception.Message);
            result.Details.Should().BeNull();

            Console.WriteLine(JsonConvert.SerializeObject(result));
        }

        [Test]
        public void when_converting_CommandException_to_Error()
        {
            var exception = new FakeCommandException("message")
            {
                String = "Value",
                Int = 1,
                Bool = true,
                DateTime = DateTime.Parse("2018-07-06"),
                Guid = Guid.Parse("3B10C34C-D423-4EC3-8811-DA2E0606E241"),
                NullableDouble = 2.1,
                Array = new[] { 1, 2 },
                IEnumerable = new[] { 3, 4 },
                List = new List<int> { 5, 6 },
                Enum = FakeEnum.Some
            };

            var result = exception.ToError();

            result.Message.Should().Be(exception.Message);
            result.Details.Should().Contain("String", exception.String);
            result.Details.Should().Contain("Int", exception.Int);
            result.Details.Should().Contain("Bool", exception.Bool);
            result.Details.Should().Contain("DateTime", exception.DateTime);
            result.Details.Should().Contain("Guid", exception.Guid);
            result.Details.Should().Contain("NullableDouble", exception.NullableDouble);
            result.Details.Should().Contain("Array", exception.Array);
            result.Details.Should().Contain("IEnumerable", exception.IEnumerable);
            result.Details.Should().Contain("List", exception.List);
            result.Details.Should().Contain("Enum", exception.Enum);

            Console.WriteLine(JsonConvert.SerializeObject(result));

            new CommandException("").ToError().Details.Should().BeNull();

            ((Exception)exception).ToError().Details.Should().NotBeNull();
        }

        [Test]
        public void when_converting_QueryException_to_Error()
        {
            var exception = new FakeQueryException("message")
            {
                String = "Value",
                Int = 1,
                Bool = true,
                DateTime = DateTime.Parse("2018-07-06"),
                Guid = Guid.Parse("3B10C34C-D423-4EC3-8811-DA2E0606E241"),
                NullableDouble = 2.1,
                Array = new[] { 1, 2 },
                IEnumerable = new[] { 3, 4 },
                List = new List<int> { 5, 6 },
                Enum = FakeEnum.Some
            };

            var result = exception.ToError();

            result.Message.Should().Be(exception.Message);
            result.Details.Should().Contain("String", exception.String);
            result.Details.Should().Contain("Int", exception.Int);
            result.Details.Should().Contain("Bool", exception.Bool);
            result.Details.Should().Contain("DateTime", exception.DateTime);
            result.Details.Should().Contain("Guid", exception.Guid);
            result.Details.Should().Contain("NullableDouble", exception.NullableDouble);
            result.Details.Should().Contain("Array", exception.Array);
            result.Details.Should().Contain("IEnumerable", exception.IEnumerable);
            result.Details.Should().Contain("List", exception.List);
            result.Details.Should().Contain("Enum", exception.Enum);

            Console.WriteLine(JsonConvert.SerializeObject(result));

            new QueryException("").ToError().Details.Should().BeNull();

            ((Exception)exception).ToError().Details.Should().NotBeNull();
        }
    }

    public class FakeCommandException : CommandException
    {
        public string String { get; set; }
        public int Int { get; set; }
        public bool Bool { get; set; }
        public DateTime DateTime { get; set; }
        public Guid Guid { get; set; }
        public double? NullableDouble { get; set; }
        public int[] Array { get; set; }
        public IEnumerable<int> IEnumerable { get; set; }
        public List<int> List { get; set; }
        public FakeEnum Enum { get; set; }

        public FakeCommandException(string message) : base(message)
        {
        }
    }

    public class FakeQueryException : QueryException
    {
        public string String { get; set; }
        public int Int { get; set; }
        public bool Bool { get; set; }
        public DateTime DateTime { get; set; }
        public Guid Guid { get; set; }
        public double? NullableDouble { get; set; }
        public int[] Array { get; set; }
        public IEnumerable<int> IEnumerable { get; set; }
        public List<int> List { get; set; }
        public FakeEnum Enum { get; set; }

        public FakeQueryException(string message) : base(message)
        {
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FakeEnum
    {
        None,
        Some,
        All
    }
}