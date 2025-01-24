using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

using GunksAlert.Api.Models;

namespace GunksAlert.Tests.Models;

public class ForecastTests {
    [Fact]
    public void ForecastIsDeserializedSuccess() {
        string json = """
            {
                "id": 0,
                "dt": 1735401600,
                "sunrise": 1735388603,
                "sunset": 1735421616,
                "moonrise": 1735381920,
                "moonset": 1735413420,
                "moon_phase": 0.93,
                "summary": "Test summary",
                "temp": {
                    "day": 32.65,
                    "min": 29.64,
                    "max": 38.16,
                    "night": 36.73,
                    "eve": 36.07,
                    "morn": 30.96
                },
                "feels_like": {
                    "day": 29.79,
                    "night": 33.69,
                    "eve": 32.18,
                    "morn": 30.96
                },
                "pressure": 1026,
                "humidity": 99,
                "dew_point": 33.58,
                "wind_speed": 5.28,
                "wind_deg": 213,
                "wind_gust": 15.68,
                "weather": [
                    {
                        "id": 616,
                        "main": "Snow",
                        "description": "rain and snow",
                        "icon": "13d"
                    }
                ],
                "clouds": 100,
                "pop": 1,
                "rain": 0.64,
                "snow": 1.07,
                "uvi": 1.03
            }
        """;
        Forecast? forecast = JsonSerializer.Deserialize<Forecast>(json);
        Assert.NotNull(forecast);
        DateTimeOffset dt = DateTimeOffset.FromUnixTimeSeconds(1735401600);

        Assert.Equal("Test summary", forecast.Summary);
        Assert.Equal(dt, forecast.Date);
        Assert.Equal(29.64, forecast.TempLow);
        Assert.Equal(38.16, forecast.TempHigh);
        Assert.Equal(29.79, forecast.TempFeelsLikeDay);

        Assert.Equal(99, forecast.Humidity);
        Assert.Equal(5.28, forecast.WindSpeed);
        Assert.Equal(15.68, forecast.WindGust);
        Assert.Equal(213, forecast.WindDegree);
        Assert.Equal(100, forecast.Clouds);
        Assert.Equal(1, forecast.Pop);
        Assert.Equal(0.64, forecast.Rain);
        Assert.Equal(1.07, forecast.Snow);

        Assert.NotNull(forecast.DailyCondition);
        Assert.Equal(616, forecast.DailyConditionId);
        Assert.Equal(616, forecast.DailyCondition.Id);
        Assert.Equal("Snow", forecast.DailyCondition.Main);
        Assert.Equal("rain and snow", forecast.DailyCondition.Description);
        Assert.Equal("13d", forecast.DailyCondition.IconDay);
        Assert.Equal("13n", forecast.DailyCondition.IconNight);
    }

    [Fact]
    public void ForecastListIsDeserializedSuccess() {
        JsonNode root = JsonNode.Parse(GetJsonData())!;
        JsonNode forecastsNode = root!["daily"]!;
        Forecast[]? forecasts = JsonSerializer.Deserialize<Forecast[]>(forecastsNode);

        Assert.NotNull(forecasts);
        Assert.Equal(8, forecasts.Length);
    }

    [Fact]
    public void ForecastIsSerializedSuccess() {
        DateTimeOffset now = new DateTimeOffset();
        Forecast.Temperature temp = new Forecast.Temperature() {
            Low = 10.1,
            High = 20.2
        };
        Forecast.FeelsLikeTemperature feelsLike = new Forecast.FeelsLikeTemperature() {
            Day = 18.8
        };
        DailyCondition condition = new DailyCondition() {
            Main = "Cloudy",
            Description = "Kinda cloudy",
            IconDay = "12d",
            IconNight = "12d"
        };
        Forecast forecast = new Forecast() {
            Summary = "Test summary",
            Date = now,
            Temp = temp,
            FeelsLike = feelsLike,
            WindSpeed = 5.0,
            WindGust = 8.5,
            WindDegree = 270,
            Clouds = 90,
            Humidity = 75,
            Pop = 50,
            Rain = 0.1,
            Snow = 0.2,
            DailyCondition = condition
        };
        string json = JsonSerializer.Serialize<Forecast>(forecast);
        long dt = now.ToUnixTimeSeconds();

        Assert.Contains("\"summary\":\"Test summary\"", json);
        Assert.Contains($"\"dt\":{dt}", json);
        Assert.Contains("\"min\":10.1", json);
        Assert.Contains("\"max\":20.2", json);
        Assert.Contains("\"day\":18.8", json);
        Assert.Contains("\"wind_speed\":5", json);
        Assert.Contains("\"wind_gust\":8.5", json);
        Assert.Contains("\"wind_deg\":270", json);
        Assert.Contains("\"clouds\":90", json);
        Assert.Contains("\"humidity\":75", json);
        Assert.Contains("\"pop\":50", json);
        Assert.Contains("\"rain\":0.1", json);
        Assert.Contains("\"snow\":0.2", json);
        Assert.Contains("\"main\":\"Cloudy\"", json);
        Assert.Contains("\"description\":\"Kinda cloudy\"", json);
        Assert.Contains("\"icon\":\"12d\"", json);
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
