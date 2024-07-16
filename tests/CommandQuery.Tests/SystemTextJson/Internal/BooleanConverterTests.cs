using System.Text.Json;
using CommandQuery.SystemTextJson;
using FluentAssertions;

namespace CommandQuery.Tests.SystemTextJson.Internal
{
    public class BooleanConverterTests
    {
        [Test]
        public void Read()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BooleanConverter());

            JsonSerializer.Deserialize<bool>("true", options).Should().BeTrue();
            JsonSerializer.Deserialize<bool>(@"""true""", options).Should().BeTrue();
            JsonSerializer.Deserialize<bool>("false", options).Should().BeFalse();
            JsonSerializer.Deserialize<bool>(@"""false""", options).Should().BeFalse();

            Action act = () => JsonSerializer.Deserialize<bool>("1", options);
            act.Should().Throw<JsonException>();
            act = () => JsonSerializer.Deserialize<bool>(@"""1""", options);
            act.Should().Throw<JsonException>();
        }

        [Test]
        public void Write()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new BooleanConverter());

            JsonSerializer.Serialize(true, options).Should().Be("true");
            JsonSerializer.Serialize(false, options).Should().Be("false");
        }
    }
}
