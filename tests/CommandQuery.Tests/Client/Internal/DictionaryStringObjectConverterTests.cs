using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommandQuery.Client;
using FluentAssertions;
using NUnit.Framework;

namespace CommandQuery.Tests.Client.Internal
{
    public class DictionaryStringObjectConverterTests
    {
        [Test]
        public async Task Read()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(stream, options, CancellationToken.None);
            result.Should().BeEmpty();

            stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"BooleanTrue\":true,\"BooleanFalse\":false,\"Byte\":1,\"Char\":\"C\",\"DateTime\":\"2021-07-09T18:37:53.2473503\",\"DateTimeOffset\":\"2021-07-09T20:39:07.8226113+02:00\",\"Decimal\":2.1,\"Double\":3.1,\"Enum\":5,\"Guid\":\"f8fe9091-dffd-4e33-8017-221554fe242f\",\"Int16\":4,\"Int32\":5,\"Int64\":6,\"SByte\":7,\"Single\":8.1,\"String\":\"String\",\"UInt16\":9,\"UInt32\":10,\"UInt64\":11,\"Uri\":\"https://github.com/hlaueriksson/CommandQuery\",\"Null\":null,\"Array\":[1,2,3],\"Dictionary\":{\"Array\":[1,2,3]}}"));
            result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(stream, options, CancellationToken.None);
            result.Should().BeEquivalentTo(
                new Dictionary<string, object>
                {
                    { "BooleanTrue", true },
                    { "BooleanFalse", false },
                    { "Byte", (byte)1 },
                    { "Char", "C" },
                    { "DateTime", DateTime.Parse("2021-07-09T18:37:53.2473503") },
                    { "DateTimeOffset", DateTime.Parse("2021-07-09T20:39:07.8226113+02:00") },
                    { "Decimal", 2.1M },
                    { "Double", 3.1 },
                    { "Enum", (int)DayOfWeek.Friday },
                    { "Guid", Guid.Parse("F8FE9091-DFFD-4E33-8017-221554FE242F") },
                    { "Int16", (short)4 },
                    { "Int32", 5 },
                    { "Int64", 6L },
                    { "SByte", (sbyte)7 },
                    { "Single", 8.1f },
                    { "String", "String" },
                    //{ "TimeSpan", TimeSpan.Parse("1:02:03:04") },
                    { "UInt16", (ushort)9 },
                    { "UInt32", 10U },
                    { "UInt64", 11UL },
                    { "Uri", "https://github.com/hlaueriksson/CommandQuery" },
                    //{ "Version", new Version("1.2.3.4") },
                    { "Null", null },
                    { "Array", new[] { 1, 2, 3 } },
                    {
                        "Dictionary",
                        new Dictionary<string, object>
                        {
                            { "Array", new[] { 1, 2, 3 } }
                        }
                    }
                });
        }

        [Test]
        public void Write()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var dict = new Dictionary<string, object>();
            var result = JsonSerializer.Serialize(dict, options);
            result.Should().Be("{}");

            dict = new Dictionary<string, object>
            {
                { "BooleanTrue", true },
                { "BooleanFalse", false },
                { "Byte", (byte)1 },
                { "Char", 'C' },
                { "DateTime", DateTime.Parse("2021-07-09T18:37:53.2473503") },
                { "DateTimeOffset", DateTimeOffset.Parse("2021-07-09T20:39:07.8226113+02:00") },
                { "Decimal", 2.1M },
                { "Double", 3.1 },
                { "Enum", DayOfWeek.Friday },
                { "Guid", Guid.Parse("F8FE9091-DFFD-4E33-8017-221554FE242F") },
                { "Int16", (short)4 },
                { "Int32", 5 },
                { "Int64", 6L },
                { "SByte", (sbyte)7 },
                { "Single", 8.1f },
                { "String", "String" },
                //{ "TimeSpan", TimeSpan.Parse("1:02:03:04") },
                { "UInt16", (ushort)9 },
                { "UInt32", 10U },
                { "UInt64", 11UL },
                { "Uri", "https://github.com/hlaueriksson/CommandQuery" },
                //{ "Version", new Version("1.2.3.4") },
                { "Null", null },
                { "Array", new[] { 1, 2, 3 } },
                {
                    "Dictionary",
                    new Dictionary<string, object>
                    {
                        { "Array", new[] { 1, 2, 3 } }
                    }
                }
            };
            result = JsonSerializer.Serialize(dict, options);
            result.Should().Be("{\"BooleanTrue\":true,\"BooleanFalse\":false,\"Byte\":1,\"Char\":\"C\",\"DateTime\":\"2021-07-09T18:37:53.2473503\",\"DateTimeOffset\":\"2021-07-09T20:39:07.8226113+02:00\",\"Decimal\":2.1,\"Double\":3.1,\"Enum\":5,\"Guid\":\"f8fe9091-dffd-4e33-8017-221554fe242f\",\"Int16\":4,\"Int32\":5,\"Int64\":6,\"SByte\":7,\"Single\":8.1,\"String\":\"String\",\"UInt16\":9,\"UInt32\":10,\"UInt64\":11,\"Uri\":\"https://github.com/hlaueriksson/CommandQuery\",\"Null\":null,\"Array\":[1,2,3],\"Dictionary\":{\"Array\":[1,2,3]}}");
        }
    }
}
