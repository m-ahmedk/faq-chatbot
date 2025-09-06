using System.Text.Json;
using System.Text.Json.Serialization;
using FAQDemo.API.Helpers;

namespace FAQDemo.API.Converters
{
    public class CleanStringJsonConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();
            return InputSanitizer.Clean(value);
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            if (!string.IsNullOrWhiteSpace(value))
                value = InputSanitizer.Clean(value); // output the same cleaning logic

            writer.WriteStringValue(value);
        }
    }
}
