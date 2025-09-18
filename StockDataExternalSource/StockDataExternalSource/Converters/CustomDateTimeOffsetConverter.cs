using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockDataExternalSource.Converters
{
    public class CustomDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        private readonly string _format;

        public CustomDateTimeOffsetConverter(string format)
        {
            _format = format;
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTimeOffset.ParseExact(reader.GetString(), _format, null);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
}
