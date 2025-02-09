using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using GunksAlert.Api.Services.Converters;
using GunksAlert.Api.Services.Attributes;

namespace GunksAlert.Api.Models;

/// <summary>
/// Stores details behind a why a day is considered climbable or not.
/// </summary>
public class ClimbabilityReport {
    public const double AcceptableDryness = 0.97;

    [Key]
    [JsonPropertyName("id")]
    public int Id { get; private set; }

    [ForeignKey("Crag")]
    [JsonIgnore]
    [NonZero]
    public int CragId { get; set; }

    [Required]
    [JsonPropertyName("date")]
    [JsonConverter(typeof(DateOnlyStringConverter))]
    public DateOnly Date { get; set; }

    [Required]
    [JsonPropertyName("temp_good")]
    public bool TempGood { get; set; }

    [Required]
    [JsonPropertyName("wind_good")]
    public bool WindGood { get; set; }

    [Required]
    [JsonPropertyName("clouds_good")]
    public bool CloudsGood { get; set; }

    [Required]
    [JsonPropertyName("humidity_good")]
    public bool HumidityGood { get; set; }

    [Required]
    [JsonPropertyName("chance_dry")]
    public double ChanceDry { get; set; }

    public string Summary() {
        string dateStr = Date.ToString("yyyy-MM-dd");
        string tempGood = TempGood ? "temperature acceptable" : "temperature not acceptable";
        string windGood = WindGood ? "wind acceptable" : "wind not acceptable";
        
        return $"{dateStr}: {tempGood}, {windGood}";
    }

    public bool IsClimbable() {
        bool isClimbable = true;
        if (!TempGood) {
            isClimbable = false;
        }

        if (ChanceDry < AcceptableDryness) {
            isClimbable = false;
        }

        return isClimbable;
    }
}
