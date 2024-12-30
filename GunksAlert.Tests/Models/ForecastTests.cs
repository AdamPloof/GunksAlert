using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

using GunksAlert.Models;

namespace GunksAlert.Tests.Models;

public class ForecastTests {
    [Fact]
    public void ForecastIsDeserializedSuccess() {
        JsonNode root = JsonNode.Parse(GetJsonData())!;
        JsonNode forecastsNode = root!["daily"]!;
        Forecast[]? forecasts = JsonSerializer.Deserialize<Forecast[]>(forecastsNode);

        Assert.NotNull(forecasts);
        Assert.Equal(8, forecasts.Length);
    }

    [Fact]
    public void ForecastIsSerializedSuccess() {
        JsonNode root = JsonNode.Parse(GetJsonData())!;
        JsonNode forecastsNode = root!["daily"]!;
        JsonArray forecastsArr = forecastsNode.AsArray();
        JsonNode forecastOneNode = forecastsArr[0]!;
        Forecast? forecast = JsonSerializer.Deserialize<Forecast>(forecastOneNode);

        Assert.NotNull(forecast);

        string originalForecastData = forecastOneNode.ToJsonString();
        string serializedForecast = JsonSerializer.Serialize(forecast);

        Assert.Equal(originalForecastData, serializedForecast);
    }

    private string GetJsonData() {
        string dataPath = Path.Combine(
            AppContext.BaseDirectory,
            "var",
            "sample_data",
            "forecast_sample.json"
        );
        if (!File.Exists(dataPath)) {
            throw new FileNotFoundException($"Sample forecast data does not exist {dataPath}");
        }

        return File.ReadAllText(dataPath);
    }
}
