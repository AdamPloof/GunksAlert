using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using GunksAlert.Models;

namespace GunksAlert.Services.Converters;

/// <summary>
/// Deserialize the `weather` property from a weather forecast/history response and 
/// return only the ID. All `DailyCondition`s are preloaded in the database so it is
/// not necessary to deserialize the entire object.
/// </summary>
/// <remarks>
/// NOTE: It is possible to meet more than one weather condition for a requested location.
/// The first weather condition in API respond is primary. That's why we're deserializing an
/// array in `Read()`
/// </remarks>
public class DailyConditionConverter : JsonConverter<DailyCondition?> {
    public override DailyCondition? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) {
        if (reader.TokenType == JsonTokenType.StartArray) {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader)) {
                JsonElement root = doc.RootElement;
                if (
                    root.GetArrayLength() > 0 
                    && root[0].TryGetProperty("id", out JsonElement prop)
                ) {
                    return JsonSerializer.Deserialize<DailyCondition>(root[0]);
                }
            }
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, DailyCondition? value, JsonSerializerOptions options) {
        if (value == null) {
            return;
        }

        JsonSerializer.Serialize(writer, value, options);
    }
}
