using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GunksAlert.Api.Services.Converters;

public class UnixTimestampConverter : JsonConverter<DateTimeOffset> {
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) {
        return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options
    ) {
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}
