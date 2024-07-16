using CommandQuery.Client;
using FluentAssertions;

namespace CommandQuery.Tests.Client.Internal
{
    public class QueryExtensionsTests
    {
        [Test]
        public void GetRequestUri_with_complex_query()
        {
            var result = TestData.FakeComplexQuery.GetRequestUri();
            result.Should()
                .StartWith("FakeComplexQuery?").And

                .Contain("Boolean=True").And
                .Contain("Byte=1").And
                .Contain("Char=C").And
                .Contain("DateOnly=07%2F09%2F2021").And
                .Contain("DateTime=2021-07-09T18%3A37%3A53.2473503").And
                .Contain("DateTimeOffset=2021-07-09T20%3A39%3A07.8226113%2B02%3A00").And
                .Contain("Decimal=2.1").And
                .Contain("Double=3.1").And
                .Contain("Enum=Friday").And
                .Contain("Guid=f8fe9091-dffd-4e33-8017-221554fe242f").And
                .Contain("Int16=4").And
                .Contain("Int32=5").And
                .Contain("Int64=6").And
                .Contain("SByte=7").And
                .Contain("Single=8").And
                .Contain("String=String").And
                .Contain("TimeOnly=18%3A37").And
                .Contain("TimeSpan=1.02%3A03%3A04").And
                .Contain("UInt16=9").And
                .Contain("UInt32=10").And
                .Contain("UInt64=11").And
                .Contain("Uri=https%3A%2F%2Fgithub.com%2Fhlaueriksson%2FCommandQuery").And
                .Contain("Version=1.2.3.4").And

                .Contain("Nullable=12").And
                //.Contain("Tuple=(13,14)").And

                .Contain("Array=15&Array=16").And
                //.Contain("IDictionary=17:18").And
                .Contain("IEnumerable=19&IEnumerable=20").And
                .Contain("IList=21&IList=22").And
                .Contain("IReadOnlyList=23&IReadOnlyList=24");

            Action act = () => ((object)null).GetRequestUri();
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void GetRequestUri_with_datetime_query()
        {
            var result = TestData.FakeDateTimeQuery.GetRequestUri();
            result.Should()
                .Contain("DateTimeUnspecified=2021-07-10T09%3A48%3A41.0000000").And
                .Contain("DateTimeUtc=2021-07-10T09%3A48%3A41.0000000Z").And
                //.Contain("DateTimeLocal=2021-07-10T09%3A48%3A41.0000000%2B02%3A00").And
                .Contain("DateTimeArray=2021-07-10T09%3A48%3A41.0000000&DateTimeArray=2021-07-10T09%3A48%3A41.0000000Z"/*&DateTimeArray=2021-07-10T09%3A48%3A41.0000000%2B02%3A00"*/);

            Action act = () => ((object)null).GetRequestUri();
            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void GetRequestUri_with_nested_query()
        {
            var result = TestData.FakeNestedQuery.GetRequestUri();
            result.Should().Be("FakeNestedQuery?Foo=Bar&Child.Foo=Bar&Child.Child.Foo=Bar");
        }

        [Test]
        public void GetRequestSlug()
        {
            new FakeComplexQuery().GetRequestSlug().Should().Be("FakeComplexQuery");

            Action act = () => ((object)null).GetRequestSlug();
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
