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
public class DailyConditionIdConverter : JsonConverter<int> {
    public override int Read(
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
                    return prop.GetInt32();
                }
            }
        }

        // ID not found!
        return 0;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) {
        // TODO: return the DailyCondition for the ID
        throw new NotImplementedException("Serialization not supported");
    }
}
