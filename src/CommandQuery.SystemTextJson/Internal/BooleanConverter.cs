using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CommandQuery.SystemTextJson
{
    internal sealed class BooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (bool.TryParse(stringValue, out bool value))
                    {
                        return value;
                    }

                    break;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}
