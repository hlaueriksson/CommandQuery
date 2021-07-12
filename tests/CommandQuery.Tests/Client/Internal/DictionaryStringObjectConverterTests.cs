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
    // https://github.com/joseftw/JOS.SystemTextJsonDictionaryStringObjectConverter/blob/develop/test/JOS.SystemTextJsonDictionaryObjectModelBinder.Tests/DictionaryStringObjectConverterTests.cs
    public class DictionaryStringObjectConverterTests
    {
        [Test]
        public async Task GivenEmptyJsonObject_WhenDeserializeAsync_ThenReturnsEmptyDictionary()
        {
            var jsonString = "{}";
            var json = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(json, options, CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GivenObjectWithStringPropertyWithDateTimeValue_WhenDeserializeAsync_ThenResultContainsSaidPropertyAsDateTime()
        {
            var jsonString = "{\"name\": \"2020-01-23T01:02:03Z\"}";
            var json = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(json, options, CancellationToken.None);

            result.Should().Contain("name", new DateTime(2020, 1, 23, 1, 2, 3, DateTimeKind.Utc));
        }

        [Test]
        public async Task GivenObjectWithIntProperty_WhenDeserializeAsync_ThenResultContainsSaidPropertyAsLong()
        {
            var jsonString = "{\"name\": 1}";
            var json = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(json, options, CancellationToken.None);

            result.Should().Contain("name", 1L);
        }

        [Test]
        public async Task GivenObjectWithDecimalProperty_WhenDeserializeAsync_ThenResultContainsSaidPropertyAsDecimal()
        {
            var jsonString = "{\"name\": 1.234}";
            var json = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(json, options, CancellationToken.None);

            result.Should().Contain("name", 1.234M);
        }

        [Test]
        public async Task GivenObjectWithBoolProperty_WhenDeserializeAsync_ThenResultContainsSaidPropertyAsBool()
        {
            var jsonString = "{\"name\": true}";
            var json = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(json, options, CancellationToken.None);

            result.Should().Contain("name", true);
        }

        [Test]
        public async Task GivenObjectWithNullPropertyValue_WhenDeserializeAsync_ThenResultContainsSaidPropertyWithNullValue()
        {
            var jsonString = "{\"name\": null}";
            var json = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(json, options, CancellationToken.None);

            result.Should().Contain("name", null);
        }

        [Test]
        public async Task GivenObjectWithArrayProperty_WhenDeserializeAsync_ThenResultContainsSaidPropertyWithArrayValue()
        {
            var jsonString = "{\"name\": [1,2,3]}";
            var json = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(json, options, CancellationToken.None);

            result.Should().ContainKey("name");
            var array = (List<object>)result["name"];
            array.Count.Should().Be(3);
            array.Should().Contain(1L);
            array.Should().Contain(2L);
            array.Should().Contain(3L);
        }

        [Test]
        public async Task GivenObjectWithObjectProperty_WhenDeserializeAsync_ThenResultContainsSaidPropertyWithNestedDictionary()
        {
            var jsonString = "{\"name\": {\"property\": 100}}";
            var json = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(json, options, CancellationToken.None);

            result.Should().ContainKey("name");
            var nestedObject = (Dictionary<string, object>)result["name"];
            nestedObject.Count.Should().Be(1);
            nestedObject["property"].Should().Be(100L);
        }

        [Test]
        public async Task GivenObjectPropertyWithArrayProperty_WhenDeserializeAsync_ThenResultContainsSaidPropertyWithNestedDictionaryAndArrayProperty()
        {
            var jsonString = "{\"name\": {\"property\": [1,2,3]}}";
            var json = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var options = new JsonSerializerOptions
            {
                Converters = { new DictionaryStringObjectConverter() }
            };

            var result = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(json, options, CancellationToken.None);

            result.Should().ContainKey("name");
            var nestedObject = (Dictionary<string, object>)result["name"];
            nestedObject.Should().ContainKey("property");
            var array = (List<object>)nestedObject["property"];
            array.Count.Should().Be(3);
            array.Should().Contain(1L);
            array.Should().Contain(2L);
            array.Should().Contain(3L);
        }
    }
}
