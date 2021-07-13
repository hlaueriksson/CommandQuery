using System.Text.Json;
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
            void should_return_an_object()
            {
                "{}".SafeDeserialize(typeof(object), null).Should().NotBeNull();

                JsonSerializer.Serialize(TestData.FakeComplexQuery)
                    .SafeDeserialize(typeof(FakeComplexQuery), null)
                    .Should().BeEquivalentTo(TestData.FakeComplexQuery);

                JsonSerializer.Serialize(TestData.FakeDateTimeQuery)
                    .SafeDeserialize(typeof(FakeDateTimeQuery), null)
                    .Should().BeEquivalentTo(TestData.FakeDateTimeQuery);

                JsonSerializer.Serialize(TestData.FakeNestedQuery)
                    .SafeDeserialize(typeof(FakeNestedQuery), null)
                    .Should().BeEquivalentTo(TestData.FakeNestedQuery);
            }

            void should_return_null_if_deserialization_fails() => ((string)null).SafeDeserialize(typeof(object), null).Should().BeNull();
        }
    }
}
