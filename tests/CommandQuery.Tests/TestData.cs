using System;
using System.Collections.Generic;

namespace CommandQuery.Tests
{
    public static class TestData
    {
        public static readonly FakeComplexQuery FakeComplexQuery = new()
        {
            Boolean = true,
            Byte = 1,
            Char = 'C',
            DateTime = DateTime.Parse("2021-07-09T18:37:53.2473503"),
            DateTimeOffset = DateTimeOffset.Parse("2021-07-09T20:39:07.8226113+02:00"),
            Decimal = 2.1M,
            Double = 3.1,
            Enum = DayOfWeek.Friday,
            Guid = Guid.Parse("F8FE9091-DFFD-4E33-8017-221554FE242F"),
            Int16 = 4,
            Int32 = 5,
            Int64 = 6L,
            SByte = 7,
            Single = 8.1f,
            String = "String",
            //TimeSpan = TimeSpan.Parse("1:02:03:04"),
            UInt16 = 9,
            UInt32 = 10U,
            UInt64 = 11UL,
            Uri = new Uri("https://github.com/hlaueriksson/CommandQuery"),
            //Version = Version.Parse("1.2.3.4"),

            Nullable = 12,

            Array = new[] { 13, 14 },
            IEnumerable = new[] { 15, 16 },
            IList = new[] { 17, 18 },
            IReadOnlyList = new[] { 19, 20 },
        };

        public static readonly Dictionary<string, object> FakeComplexQuery_As_Dictionary_Of_String_Object = new()
        {
            { "Boolean", "true" },
            { "Byte", "1" },
            { "Char", "C" },
            { "DateTime", "2021-07-09T18:37:53.2473503" },
            { "DateTimeOffset", "2021-07-09T20:39:07.8226113+02:00" },
            { "Decimal", "2.1" },
            { "Double", "3.1" },
            { "Enum", "Friday" },
            { "Guid", "F8FE9091-DFFD-4E33-8017-221554FE242F" },
            { "Int16", "4" },
            { "Int32", "5" },
            { "Int64", "6" },
            { "SByte", "7" },
            { "Single", "8.1" },
            { "String", "String" },
            //{ "TimeSpan", "1:02:03:04" },
            { "UInt16", "9" },
            { "UInt32", "10" },
            { "UInt64", "11" },
            { "Uri", "https://github.com/hlaueriksson/CommandQuery" },
            //{ "Version", "1.2.3.4" },

            { "Nullable", "12" },

            { "Array", new[] { "13", "14" } },
            { "IEnumerable", new[] { "15", "16" } },
            { "IList", new[] { "17", "18" } },
            { "IReadOnlyList", new[] { "19", "20" } },

            { "UndefinedProperty", "should_not_be_used" },
        };

        public static readonly Dictionary<string, IEnumerable<string>> FakeComplexQuery_As_Dictionary_Of_String_IEnumerable_String = new()
        {
            { "Boolean", new[] { "true" } },
            { "Byte", new[] { "1" } },
            { "Char", new[] { "C" } },
            { "DateTime", new[] { "2021-07-09T18:37:53.2473503" } },
            { "DateTimeOffset", new[] { "2021-07-09T20:39:07.8226113+02:00" } },
            { "Decimal", new[] { "2.1" } },
            { "Double", new[] { "3.1" } },
            { "Enum", new[] { "Friday" } },
            { "Guid", new[] { "F8FE9091-DFFD-4E33-8017-221554FE242F" } },
            { "Int16", new[] { "4" } },
            { "Int32", new[] { "5" } },
            { "Int64", new[] { "6" } },
            { "SByte", new[] { "7" } },
            { "Single", new[] { "8.1" } },
            { "String", new[] { "String" } },
            //{ "TimeSpan", new[] { "1:02:03:04,0000005" } },
            { "UInt16", new[] { "9" } },
            { "UInt32", new[] { "10" } },
            { "UInt64", new[] { "11" } },
            { "Uri", new[] { "https://github.com/hlaueriksson/CommandQuery" } },
            //{ "Version", new[] { "1.2.3.4" } },

            { "Nullable", new[] { "12" } },

            { "Array", new[] { "13", "14" } },
            { "IEnumerable", new[] { "15", "16" } },
            { "IList", new[] { "17", "18" } },
            { "IReadOnlyList", new[] { "19", "20" } },

            { "UndefinedProperty", new[] { "should_not_be_used" } },
        };

        public static readonly FakeDateTimeQuery FakeDateTimeQuery = new()
        {
            DateTimeUnspecified = new DateTime(2021, 7, 10, 9, 48, 41, DateTimeKind.Unspecified),
            DateTimeUtc = new DateTime(2021, 7, 10, 9, 48, 41, DateTimeKind.Utc),
            //DateTimeLocal = new DateTime(2021, 7, 10, 9, 48, 41, DateTimeKind.Local),
            DateTimeArray = new[]
            {
                new DateTime(2021, 7, 10, 9, 48, 41, DateTimeKind.Unspecified),
                new DateTime(2021, 7, 10, 9, 48, 41, DateTimeKind.Utc),
                //new DateTime(2021, 7, 10, 9, 48, 41, DateTimeKind.Local),
            }
        };

        public static readonly Dictionary<string, object> FakeDateTimeQuery_As_Dictionary_Of_String_Object = new()
        {
            { "DateTimeUnspecified", "2021-07-10T09:48:41.0000000" },
            { "DateTimeUtc", "2021-07-10T09:48:41.0000000Z" },
            //{ "DateTimeLocal", "2021-07-10T09:48:41.0000000+02:00" },
            { "DateTimeArray", new[] { "2021-07-10T09:48:41.0000000", "2021-07-10T09:48:41.0000000Z", /*"2021-07-10T09:48:41.0000000+02:00"*/ } },
        };

        public static readonly Dictionary<string, IEnumerable<string>> FakeDateTimeQuery_As_Dictionary_Of_String_IEnumerable_String = new()
        {
            { "DateTimeUnspecified", new[] { "2021-07-10T09:48:41.0000000" } },
            { "DateTimeUtc", new[] { "2021-07-10T09:48:41.0000000Z" } },
            //{ "DateTimeLocal", new[] { "2021-07-10T09:48:41.0000000+02:00" } },
            { "DateTimeArray", new[] { "2021-07-10T09:48:41.0000000", "2021-07-10T09:48:41.0000000Z", /*"2021-07-10T09:48:41.0000000+02:00"*/ } },
        };

        public static readonly FakeNestedQuery FakeNestedQuery = new()
        {
            Foo = "Bar",
            Child = new FakeNestedChild
            {
                Foo = "Bar",
                Child = new FakeNestedGrandchild
                {
                    Foo = "Bar"
                }
            }
        };

        public static readonly Dictionary<string, object> FakeNestedQuery_As_Dictionary_Of_String_Object = new()
        {
            { "Foo", "Bar" },
            { "Child", new Dictionary<string, object>
                {
                    { "Foo", "Bar" },
                    { "Child", new Dictionary<string, object>
                        {
                            { "Foo", "Bar" }
                        }
                    }
                }
            }
        };

        public static readonly Dictionary<string, IEnumerable<string>> FakeNestedQuery_As_Dictionary_Of_String_IEnumerable_String = new()
        {
            { "Foo", new[] { "Bar" } },
            { "Child.Foo", new[] { "Bar" } },
            { "Child.Child.Foo", new[] { "Bar" } },
        };
    }
}
