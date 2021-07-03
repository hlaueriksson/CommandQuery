using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CommandQuery.SystemTextJson.Internal
{
    internal sealed class BooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (value is null)
            {
                return false;
            }

            if (value.ToLower().Equals("true", StringComparison.Ordinal))
            {
                return true;
            }

            if (value.ToLower().Equals("false", StringComparison.Ordinal))
            {
                return false;
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            switch (value)
            {
                case true:
                    writer.WriteStringValue("true");
                    break;
                case false:
                    writer.WriteStringValue("false");
                    break;
            }
        }
    }
}
