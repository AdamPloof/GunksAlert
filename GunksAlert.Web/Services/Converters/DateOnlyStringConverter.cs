using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GunksAlert.Services.Converters;

public class DateOnlyStringConverter : JsonConverter<DateOnly> {
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) {
        if (reader.TokenType != JsonTokenType.String) {
            throw new JsonException($"Could not parse DateOnly. Expected string, got: {reader.TokenType}");
        }

        string dateString = reader.GetString()!;
        if (
            DateOnly.TryParseExact(
                dateString,
                DateFormat,
                null,
                System.Globalization.DateTimeStyles.None,
                out var date
        )) {
            return date;
        }

        throw new JsonException($"Invalid date format. Expected {DateFormat}, got {dateString}");
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateOnly value,
        JsonSerializerOptions options
    ) {
        writer.WriteStringValue(value.ToString(DateFormat));
    }
}
