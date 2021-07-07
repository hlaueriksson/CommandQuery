using CommandQuery.SystemTextJson;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.SystemTextJson.Internal
{
    public class JsonExtensionsTests
    {
        [LoFu, Test]
        public void SafeDeserialize()
        {
            void should_return_an_object() => "{}".SafeDeserialize(typeof(object)).Should().NotBeNull();

            void should_return_null_if_deserialization_fails() => ((string)null).SafeDeserialize(typeof(object)).Should().BeNull();
        }
    }
}
