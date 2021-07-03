using System;
using System.Collections.Generic;
using CommandQuery.SystemTextJson.Internal;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.SystemTextJson.Internal
{
    public class DictionaryExtensionsTests
    {
        [LoFu, Test]
        public void when_converting_a_dictionary_to_object()
        {
            void should_set_the_property_values()
            {
                var subject = new Dictionary<string, object>
                {
                    { "String", "Value" },
                    { "Int", "1" },
                    { "Bool", "true" },
                    { "DateTime", "2018-07-06" },
                    { "Guid", "3B10C34C-D423-4EC3-8811-DA2E0606E241" },
                    { "NullableDouble", "2.1" },
                    { "UndefinedProperty", "should_not_be_used" },
                    { "Array", new[] { "1", "2" } },
                    { "IEnumerable", new[] { "3", "4" } },
                    { "List", new[] { "5", "6" } }
                };

                var result = subject.SafeToObject(typeof(FakeComplexQuery)) as FakeComplexQuery;

                result.String.Should().Be("Value");
                result.Int.Should().Be(1);
                result.Bool.Should().Be(true);
                result.DateTime.Should().Be(DateTime.Parse("2018-07-06"));
                result.Guid.Should().Be(new Guid("3B10C34C-D423-4EC3-8811-DA2E0606E241"));
                result.NullableDouble.Should().Be(2.1);
                result.Array.Should().Equal(1, 2);
                result.IEnumerable.Should().Equal(3, 4);
                result.List.Should().Equal(5, 6);
            }

            void should_return_null_if_dictionary_is_null()
            {
                IDictionary<string, object> subject = null;

                subject.SafeToObject(typeof(FakeComplexQuery)).Should().BeNull();
            }

            void should_return_null_if_conversion_fails()
            {
                var subject = new Dictionary<string, object>
                {
                    { "Guid", "fail" }
                };

                subject.SafeToObject(typeof(FakeComplexQuery)).Should().BeNull();
            }
        }
    }
}
