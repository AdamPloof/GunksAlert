using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using GunksAlert.Services.Converters;
using GunksAlert.Services.Attributes;

namespace GunksAlert.Models;

/// <summary>
/// Stores data for a single day's weather forecast. Is used by comparing to `ClimbableConditions`
/// to determine if an alert should be sent.
/// </summary>
/// 
/// <remarks>
/// The <see cref="DailyConditionId"/> property reference a DailyCondition summary which is
/// a standarized short summary for the day's weather.
/// </remarks>
public class Forecast {
    private Temperature _temp = new Temperature();
    private double _tempLow;
    private double _tempHigh;
    private FeelsLikeTemperature _tempFeelsLike = new FeelsLikeTemperature();
    private double _tempFeelsLikeDay;

    [Key]
    [JsonPropertyName("id")]
    public int Id { get; private set; }

    [Required]
    [JsonPropertyName("dt")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTimeOffset Date { get; set; }

    [StringLength(200)]
    [JsonPropertyName("summary")]
    public required string Summary { get; set; }

    [JsonPropertyName("temp")]
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

    [JsonPropertyName("feels_like")]
    [NotMapped]
    public FeelsLikeTemperature FeelsLike {
        get => _tempFeelsLike;
        set {
            _tempFeelsLike = value;
            _tempFeelsLikeDay = value.Day;
        }
    }

    [Required]
    [JsonIgnore]
    public double TempFeelsLikeDay {
        get => _tempFeelsLikeDay;
        set {
            _tempFeelsLikeDay = value;
            _tempFeelsLike.Day = value;
        }
    }

    [Required]
    public double WindSpeed { get; set; }

    [Required]
    public double WindGust { get; set; }

    [Required]
    [Range(0, 360)]
    public int WindDegree { get; set; }

    [Required]
    [Range(0, 100)]
    public int Clouds { get; set; }

    [Required]
    [Range(0, 100)]
    public int Humidity { get; set; }

    [Required]
    [Range(0, 100)]
    [JsonPropertyName("pop")]
    public double Pop { get; set; }

    [JsonPropertyName("rain")]
    public double Rain { get; set; }

    [JsonPropertyName("snow")]
    public double Snow { get; set ;}

    [ForeignKey("DailyCondition")]
    [JsonPropertyName("weather")]
    [JsonConverter(typeof(DailyConditionIdConverter))]
    [NonZero]
    public int DailyConditionId { get; set; }

    // TODO: Set this on setting DailyConditionId
    [JsonIgnore]
    public DailyCondition? DailyCondition { get; set; }

    public class Temperature {
        [JsonPropertyName("min")]
        public double Low { get; set; }

        [JsonPropertyName("max")]
        public double High { get; set; }
    }

    public class FeelsLikeTemperature {
        [JsonPropertyName("day")]
        public double Day;
    }
}