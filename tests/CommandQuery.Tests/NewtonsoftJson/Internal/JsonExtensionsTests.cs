using CommandQuery.NewtonsoftJson;
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

            void should_return_null_if_deserialization_fails() => ((string)null).SafeToObject(typeof(object)).Should().BeNull();
        }
    }
}
