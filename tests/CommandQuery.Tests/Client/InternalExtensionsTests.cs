using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
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
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            var subject = new FakeComplexQuery
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
                .Contain("DateTime=11%2F19%2F2019+5%3A50%3A13+PM").And
                .Contain("Guid=b665da2a-60fe-4f2b-baf1-9d766e2542d3").And
                .Contain("NullableDouble=2.1").And
                .Contain("Array=1&Array=2").And
                .Contain("IEnumerable=3&IEnumerable=4").And
                .Contain("List=5&List=6");
        }
    }
}