using System;
using CommandQuery.NewtonsoftJson.Internal;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.NewtonsoftJson.Internal
{
    public class JsonExtensionsTests
    {
        [LoFu, Test]
        public void SafeToObject()
        {
            void should_return_an_object() => "{}".SafeToObject(typeof(object)).Should().NotBeNull();

            void should_return_null_if_convertion_fails() => ((string)null).SafeToObject(typeof(object)).Should().BeNull();
        }

        [LoFu, Test]
        public void ToJson()
        {
            void should_return_a_json_string() => new object().ToJson().Should().NotBeNull();

            void should_return_null_if_serialization_fails() => new Failer().ToJson().Should().BeNull();
        }

        class Failer
        {
            public string Property => throw new Exception("fail");
        }
    }
}
