using CommandQuery.NewtonsoftJson;
using FluentAssertions;
using LoFuUnit.NUnit;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CommandQuery.Tests.NewtonsoftJson.Internal
{
    public class JsonExtensionsTests
    {
        [LoFu, Test]
        public void SafeDeserialize()
        {
            void should_return_an_object()
            {
                "{}".SafeDeserialize(typeof(object), null).Should().NotBeNull();

                JsonConvert.SerializeObject(TestData.FakeComplexQuery)
                    .SafeDeserialize(typeof(FakeComplexQuery), null)
                    .Should().BeEquivalentTo(TestData.FakeComplexQuery);

                JsonConvert.SerializeObject(TestData.FakeDateTimeQuery)
                    .SafeDeserialize(typeof(FakeDateTimeQuery), null)
                    .Should().BeEquivalentTo(TestData.FakeDateTimeQuery);

                JsonConvert.SerializeObject(TestData.FakeNestedQuery)
                    .SafeDeserialize(typeof(FakeNestedQuery), null)
                    .Should().BeEquivalentTo(TestData.FakeNestedQuery);
            }

            void should_return_null_if_deserialization_fails() => ((string)null).SafeDeserialize(typeof(object), null).Should().BeNull();
        }
    }
}
