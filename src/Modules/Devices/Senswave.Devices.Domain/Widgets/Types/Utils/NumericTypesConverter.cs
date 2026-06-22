using System.Text.Json;

namespace Senswave.Devices.Domain.Widgets.Types.Utils;

public class NumericTypesConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out int intValue))
                return intValue;
            if (reader.TryGetDouble(out double doubleValue))
                return doubleValue;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case int intValue:
                writer.WriteNumberValue(intValue);
                break;
            case double doubleValue:
                writer.WriteNumberValue(doubleValue);
                break;
            default:
                writer.WriteNullValue();
                break;
        }
    }
}

