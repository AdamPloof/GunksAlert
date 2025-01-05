using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using GunksAlert.Services.Converters;

namespace GunksAlert.Models;

/// <summary>
/// Represents the weather for a single day in the past.
/// </summary>
public class WeatherHistory {
    private Temperature _temp = new Temperature();
    private double _tempLow;
    private double _tempHigh;

    private DailyCloudCover _dailyClouds = new DailyCloudCover();
    private int _clouds;

    private DailyPrecipitation _dailyPrecipitation = new DailyPrecipitation();
    private int _precipitation;

    private DailyHumidity _dailyHumidity = new DailyHumidity();
    private int _humidity;

    private MaxWind _maxWind = new MaxWind();
    private double _windSpeed;
    private int _windDegree;

    [Key]
    public int Id { get; private set; }

    [Required]
    [JsonPropertyName("date")]
    [JsonConverter(typeof(DateOnlyStringConverter))]
    public DateOnly Date { get; set; }

    [JsonPropertyName("temperature")]
    [NotMapped]
    public Temperature Temp {
        get => _temp;
        set {
            _temp = value;
            _tempLow = value.Low;
            _tempHigh = value.High;
        }
    }

    [Required]
    [JsonIgnore]
    public double TempLow {
        get => _tempLow;
        set {
            _tempLow = value;
            _temp.Low = value;
        }
    }

    [Required]
    [JsonIgnore]
    public double TempHigh {
        get => _tempHigh;
        set {
            _tempHigh = value;
            _temp.High = value;
        }
    }

    [JsonPropertyName("cloud_cover")]
    [NotMapped]
    public DailyCloudCover CloudsPercent {
        get => _dailyClouds;
        set {
            _dailyClouds = value;
            _clouds = value.Percent;
        }
    }

    [Required]
    [JsonIgnore]
    public int Clouds {
        get => _clouds;
        set {
            _clouds = value;
            _dailyClouds.Percent = value;
        }
    }

    [JsonPropertyName("humidity")]
    [NotMapped]
    public DailyHumidity HumidtyPercent {
        get => _dailyHumidity;
        set {
            _dailyHumidity = value;
            _humidity = value.Percent;
        }
    }

    [Required]
    [JsonIgnore]
    public int Humidity {
        get => _humidity;
        set {
            _humidity = value;
            _dailyHumidity.Percent = value;
        }
    }

    [JsonPropertyName("precipitation")]
    [NotMapped]
    public DailyPrecipitation PrecipitationTotal {
        get => _dailyPrecipitation;
        set {
            _dailyPrecipitation = value;
            _precipitation = value.Amount;
        }
    }

    [Required]
    [JsonIgnore]
    public int Precipitation {
        get => _precipitation;
        set {
            _precipitation = value;
            _dailyPrecipitation.Amount = value;
        }
    }

    [JsonPropertyName("wind")]
    [JsonConverter(typeof(MaxWindConverter))]
    [NotMapped]
    public MaxWind Wind {
        get => _maxWind;
        set {
            _maxWind = value;
            _windSpeed = value.Speed;
            _windDegree = value.Direction;
        }
    }

    [Required]
    [JsonIgnore]
    public double WindSpeed {
        get => _windSpeed;
        set {
            _windSpeed = value;
            _maxWind.Speed = value;
        }
    }

    [Required]
    [JsonIgnore]
    public int WindDegree {
        get => _windDegree;
        set {
            _windDegree = value;
            _maxWind.Direction = value;
        }
    }

    public class Temperature {
        [JsonPropertyName("min")]
        public double Low { get; set; }

        [JsonPropertyName("max")]
        public double High { get; set; }
    }

    public class DailyHumidity {
        [JsonPropertyName("afternoon")]
        public int Percent { get; set; }
    }

    public class DailyPrecipitation {
        [JsonPropertyName("total")]
        public int Amount { get; set; }
    }

    public class DailyCloudCover {
        [JsonPropertyName("afternoon")]
        public int Percent { get; set; }
    }

    public class MaxWind {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("direction")]
        public int Direction { get; set; }
    }
}
