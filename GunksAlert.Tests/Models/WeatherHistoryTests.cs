using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;
using NuGet.ContentModel;

using GunksAlert.Api.Models;

namespace GunksAlert.Tests.Models;

public class WeatherHistoryTests {
    [Fact]
    public void WeatherHistoryIsDeserializedSuccess() {
        string json = """
            {
                "lat":33,
                "lon":35,
                "tz":"+02:00",
                "date":"2020-03-04",
                "units":"standard",
                "cloud_cover":{
                    "afternoon":10.4
                },
                "humidity":{
                    "afternoon":33.3
                },
                "precipitation":{
                    "total":4.4
                },
                "temperature":{
                    "min":286.48,
                    "max":299.24,
                    "afternoon":296.15,
                    "night":289.56,
                    "evening":295.93,
                    "morning":287.59
                },
                "pressure":{
                    "afternoon":1015
                },
                "wind":{
                    "max":{
                        "speed":8.7,
                        "direction":120
                    }
                }
            }
        """;

        WeatherHistory? history = JsonSerializer.Deserialize<WeatherHistory>(json);
        DateOnly dt = new DateOnly(2020, 3, 4);

        Assert.NotNull(history);
        Assert.Equal(dt, history.Date);
        Assert.Equal(286.48, history.TempLow);
        Assert.Equal(299.24, history.TempHigh);
        Assert.Equal(8.7, history.WindSpeed);
        Assert.Equal(120, history.WindDegree);
        Assert.Equal(10.4, history.Clouds);
        Assert.Equal(33.3, history.Humidity);
        Assert.Equal(4.4, history.Precipitation);
    }

    [Fact]
    public void WeatherHistoryIsSerializedSuccess() {
        string DateFormat = "yyyy-MM-dd";
        if (!DateOnly.TryParseExact(
            "2024-08-12",
            DateFormat,
            null,
            System.Globalization.DateTimeStyles.None,
            out DateOnly weatherDate
        )) {
            throw new Exception("Unable to create date from format");
        }
        
        WeatherHistory history = new WeatherHistory() {
            Date = weatherDate,
            Temp = new WeatherHistory.Temperature() {Low = 10.1, High = 20.2},
            HumidtyPercent = new WeatherHistory.DailyHumidity() {Percent = 40.42},
            CloudsPercent = new WeatherHistory.DailyCloudCover() {Percent = 90.91},
            PrecipitationTotal = new WeatherHistory.DailyPrecipitation() {Amount = 2.1},
            Wind = new WeatherHistory.MaxWind() {Direction = 270.9, Speed = 15}
        };
        string json = JsonSerializer.Serialize<WeatherHistory>(history);
        string dt = weatherDate.ToString("yyyy-MM-dd");

        Assert.Contains($"\"date\":\"{dt}\"", json);
        Assert.Contains("\"min\":10.1", json);
        Assert.Contains("\"max\":20.2", json);
        Assert.Contains("\"afternoon\":40.42", json);
        Assert.Contains("\"afternoon\":90.91", json);
        Assert.Contains("\"total\":2.1", json);
        Assert.Contains("\"speed\":15", json);
        Assert.Contains("\"direction\":270.9", json);
    }
}
