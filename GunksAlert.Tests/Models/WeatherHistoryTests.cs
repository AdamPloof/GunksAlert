using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

using GunksAlert.Models;

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
                    "afternoon":10
                },
                "humidity":{
                    "afternoon":33
                },
                "precipitation":{
                    "total":4
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
        Assert.Equal(10, history.Clouds);
        Assert.Equal(33, history.Humidity);
        Assert.Equal(4, history.Precipitation);
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
            HumidtyPercent = new WeatherHistory.DailyHumidity() {Percent = 40},
            CloudsPercent = new WeatherHistory.DailyCloudCover() {Percent = 90},
            PrecipitationTotal = new WeatherHistory.DailyPrecipitation() {Amount = 2},
            Wind = new WeatherHistory.MaxWind() {Direction = 270, Speed = 15}
        };
        string json = JsonSerializer.Serialize<WeatherHistory>(history);

        Assert.Equal(weatherDate, history.Date);
        Assert.Equal(10.1, history.TempLow);
        Assert.Equal(20.2, history.TempHigh);

        Assert.Equal(40, history.Humidity);
        Assert.Equal(90, history.Clouds);
        Assert.Equal(2, history.Precipitation);
        Assert.Equal(15, history.WindSpeed);
        Assert.Equal(270, history.WindDegree);
    }
}
