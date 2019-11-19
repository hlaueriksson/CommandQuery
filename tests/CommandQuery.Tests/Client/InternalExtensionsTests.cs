using System;
using System.Collections.Generic;
using CommandQuery.Client;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.Client
{
    public class InternalExtensionsTests
    {
        [LoFu, Test]
        public void when_converting_a_dictionary_to_object()
        {
            var subject = new FakeQuery
            {
                String = "String",
                Int = 1,
                Bool = true,
                DateTime = DateTime.Parse("2019-11-19 17:50:13"),
                Guid = Guid.Parse("b665da2a-60fe-4f2b-baf1-9d766e2542d3"),
                NullableDouble = 2.1,
                Array = new []{ 1, 2 },
                IEnumerable = new []{ 3, 4 },
                List = new List<int> { 5, 6 }
            };

            var result = subject.GetRequestUri();

            result.Should()
                .Contain("String=String").And
                .Contain("Int=1").And
                .Contain("Bool=True").And
                .Contain("DateTime=2019-11-19+17%3A50%3A13").And
                .Contain("Guid=b665da2a-60fe-4f2b-baf1-9d766e2542d3").And
                .Contain("NullableDouble=2%2C1").And
                .Contain("Array=1&Array=2").And
                .Contain("IEnumerable=3&IEnumerable=4").And
                .Contain("List=5&List=6");
        }

        private class FakeQuery
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
        }
    }
}