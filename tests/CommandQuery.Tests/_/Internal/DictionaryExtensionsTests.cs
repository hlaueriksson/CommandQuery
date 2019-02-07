using System;
using System.Collections.Generic;
using CommandQuery.Internal;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests._.Internal
{
    public class DictionaryExtensionsTests
    {
        [LoFu, Test]
        public void when_converting_a_dictionary_to_object()
        {
            Subject = new Dictionary<string, string>
            {
                { "String", "Value" },
                { "Int", "1" },
                { "Bool", "true" },
                { "DateTime", "2018-07-06" },
                //{ "Guid", "3B10C34C-D423-4EC3-8811-DA2E0606E241" },
                { "NullableDouble", "2" },
                { "UndefinedProperty", "should_not_be_used" }
            };

            void should_set_the_property_values()
            {
                var result = Subject.SafeToObject(typeof(FakeQuery)) as FakeQuery;

                result.String.Should().Be("Value");
                result.Int.Should().Be(1);
                result.Bool.Should().Be(true);
                result.DateTime.Should().Be(DateTime.Parse("2018-07-06"));
                //result.Guid.Should().Be(new Guid("3B10C34C-D423-4EC3-8811-DA2E0606E241"));
                result.NullableDouble.Should().Be(2);
            }
        }

        IDictionary<string, string> Subject;

        private class FakeQuery
        {
            public string String { get; set; }
            public int Int { get; set; }
            public bool Bool { get; set; }
            public DateTime DateTime { get; set; }
            //public Guid Guid { get; set; }
            public double? NullableDouble { get; set; }
        }
    }
}