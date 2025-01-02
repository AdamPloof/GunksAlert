using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GunksAlert.Models;

/// <summary>
/// Represents a standarized weather condition summary for a day including an icon code.
/// </summary>
/// <remarks>
/// NOTE: It is possible to meet more than one weather condition for a requested location.
/// The first weather condition in API respond is primary.
/// </remarks>
/// <seealso href="https://openweathermap.org/weather-conditions" />
public class DailyCondition {
    [Key]
    public int Id { get; private set; }

    [StringLength(200)]
    [Required]
    [JsonPropertyName("main")]
    public required string Main { get; set; }

    [StringLength(200)]
    [Required]
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [StringLength(3)]
    [Required]
    [JsonPropertyName("iconDay")]
    public required string IconDay { get; set; }

    [StringLength(3)]
    [Required]
    [JsonPropertyName("iconNight")]
    public required string IconNight { get; set; }
}
