using System.Collections.Generic;
using System.Linq;
using CommandQuery.NewtonsoftJson;
using FluentAssertions;
using LoFuUnit.NUnit;
using NUnit.Framework;

namespace CommandQuery.Tests.NewtonsoftJson.Internal
{
    public class DictionaryExtensionsTests
    {
        [LoFu, Test]
        public void SafeDeserialize()
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
