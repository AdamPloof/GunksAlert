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
/// <remarks>
/// Useful for debugging why days did or did not trigger an alert.
/// </remarks>
public class ConditionsReport {
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
    public double TempFeelsLike { get; set; }

    [Required]
    public double TempMin { get; set; }

    [Required]
    public double TempMax { get; set; }

    [Required]
    public double WindSpeed { get; set; }

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
    public double EstimatedSnowpack { get; set; }

    [Required]
    public double PreciptationDayBefore { get; set; }

    [Required]
    public double PreciptationDayOf { get; set; }

    [Required]
    public double ChanceDry { get; set; }

    public bool IsClimbable() {
        bool isClimbable = true;
        if (TempFeelsLike < TempMin || TempFeelsLike > TempMax) {
            isClimbable = false;
        }

        if (ChanceDry < AcceptableDryness) {
            isClimbable = false;
        }

        return isClimbable;
    }
}
