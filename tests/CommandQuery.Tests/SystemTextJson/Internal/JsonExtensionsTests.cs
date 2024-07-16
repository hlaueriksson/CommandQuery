using System.Text.Json;
using CommandQuery.SystemTextJson;
using FluentAssertions;

namespace CommandQuery.Tests.SystemTextJson.Internal
{
    public class JsonExtensionsTests
    {
        [LoFu, Test]
        public void SafeDeserialize_string()
        {
            void should_return_an_object()
            {
                "{}".SafeDeserialize(typeof(object)).Should().NotBeNull();

                JsonSerializer.Serialize(TestData.FakeComplexQuery)
                    .SafeDeserialize(typeof(FakeComplexQuery))
                    .Should().BeEquivalentTo(TestData.FakeComplexQuery);

                JsonSerializer.Serialize(TestData.FakeDateTimeQuery)
                    .SafeDeserialize(typeof(FakeDateTimeQuery))
                    .Should().BeEquivalentTo(TestData.FakeDateTimeQuery);

                JsonSerializer.Serialize(TestData.FakeNestedQuery)
                    .SafeDeserialize(typeof(FakeNestedQuery))
                    .Should().BeEquivalentTo(TestData.FakeNestedQuery);
            }

            void should_have_sane_defaults()
            {
                var result = "{\"int32\":\"1\"}".SafeDeserialize(typeof(FakeComplexQuery)) as FakeComplexQuery;
                result.Int32.Should().Be(1);
            }

            void should_use_options_when_provided()
            {
                var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                var result = "{\"int32\":\"1\"}".SafeDeserialize(typeof(FakeComplexQuery), options) as FakeComplexQuery;
                result.Int32.Should().Be(1);
            }

            void should_return_null_if_deserialization_fails() => ((string)null).SafeDeserialize(typeof(object)).Should().BeNull();
        }

        [LoFu, Test]
        public void SafeDeserialize_Dictionary()
        {
            void should_set_the_property_values()
            {
                var subject = TestData.FakeComplexQuery_As_Dictionary_Of_String_Object;
                var result = subject.SafeDeserialize(typeof(FakeComplexQuery)) as FakeComplexQuery;
                result.Should().BeEquivalentTo(TestData.FakeComplexQuery);

                subject = TestData.FakeComplexQuery_As_Dictionary_Of_String_Object.ToDictionary(x => x.Key.ToLower(), x => x.Value);
                result = subject.SafeDeserialize(typeof(FakeComplexQuery)) as FakeComplexQuery;
                result.Should().BeEquivalentTo(TestData.FakeComplexQuery);
            }

            void should_set_the_property_values_of_DateTime_kinds()
            {
                var subject = TestData.FakeDateTimeQuery_As_Dictionary_Of_String_Object;

                var result = subject.SafeDeserialize(typeof(FakeDateTimeQuery)) as FakeDateTimeQuery;

                result.Should().BeEquivalentTo(TestData.FakeDateTimeQuery);
            }

            void should_not_set_the_property_values_of_nested_objects()
            {
                var subject = TestData.FakeNestedQuery_As_Dictionary_Of_String_Object;

                var result = subject.SafeDeserialize(typeof(FakeNestedQuery)) as FakeNestedQuery;

                result.Should().BeEquivalentTo(TestData.FakeNestedQuery);
            }

            void should_return_null_if_dictionary_is_null()
            {
                IDictionary<string, object> subject = null;

                subject.SafeDeserialize(typeof(FakeComplexQuery)).Should().BeNull();
            }

            void should_return_null_if_conversion_fails()
            {
                var subject = new Dictionary<string, object>
                {
                    { "Guid", "fail" }
                };

                subject.SafeDeserialize(typeof(FakeComplexQuery)).Should().BeNull();
            }
        }
    }
}
