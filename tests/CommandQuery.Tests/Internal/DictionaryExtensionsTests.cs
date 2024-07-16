using FluentAssertions;

namespace CommandQuery.Tests.Internal
{
    public class DictionaryExtensionsTests
    {
        [Test]
        public void GetQueryDictionary()
        {
            var query = TestData.FakeComplexQuery_As_Dictionary_Of_String_IEnumerable_String;
            var result = query.GetQueryDictionary(typeof(FakeComplexQuery));
            result.Should().BeEquivalentTo(TestData.FakeComplexQuery_As_Dictionary_Of_String_Object);
        }

        [Test]
        public void GetQueryDictionary_with_nested_query()
        {
            var query = TestData.FakeNestedQuery_As_Dictionary_Of_String_IEnumerable_String;
            var result = query.GetQueryDictionary(typeof(FakeNestedQuery));
            result.Should().BeEquivalentTo(TestData.FakeNestedQuery_As_Dictionary_Of_String_Object);
        }
    }
}
