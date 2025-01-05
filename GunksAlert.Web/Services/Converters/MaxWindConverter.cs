using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using GunksAlert.Models;

namespace GunksAlert.Services.Converters;

/// <summary>
/// In weather history data (aggregate weather) wind information is stored in 
/// a nested object. This converter extracts the wind speed and direction into
/// a WeatherHistory.MaxWind object.
/// </summary>
public class MaxWindConverter : JsonConverter<WeatherHistory.MaxWind?> {
    public override WeatherHistory.MaxWind? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    ) {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader)) {
            JsonElement max = doc.RootElement.GetProperty("max");
            double speed = max.GetProperty("speed").GetDouble();
            int direction = max.GetProperty("direction").GetInt32();
            
            return new WeatherHistory.MaxWind() {
                Speed = speed,
                Direction = direction
            };
        }
    }

    public override void Write(Utf8JsonWriter writer, WeatherHistory.MaxWind? value, JsonSerializerOptions options) {
        if (value == null) {
            return;
        }

        JsonSerializer.Serialize(writer, value, options);
    }
}
